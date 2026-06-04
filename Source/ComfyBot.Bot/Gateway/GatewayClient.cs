using System;
using System.Threading.Tasks;
using ComfyBot.Bot.Scaffolding;
using ComfyBot.Gateway.Contracts.Events;
using ComfyBot.Gateway.Contracts.Requests;
using ComfyBot.Gateway.Contracts.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ComfyBot.Bot.Gateway;

public class GatewayClient : IGatewayClient, IDisposable
{
    private readonly ILogger<GatewayClient> logger;
    private readonly BotSettings settings;
    private HubConnection? hubConnection;

    public event EventHandler<MessageReceivedEvent>? OnMessageReceived;
    public event EventHandler<CommandReceivedEvent>? OnCommandReceived;

    public GatewayClient(IOptions<BotSettings> settings, ILogger<GatewayClient> logger)
    {
        this.logger = logger;
        this.settings = settings.Value;
    }

    public async Task ConnectAsync()
    {
        string gatewayUrl = this.settings.GatewayUrl ?? "http://localhost:5125";

        this.hubConnection = new HubConnectionBuilder()
            .WithUrl($"{gatewayUrl}/chat")
            .WithAutomaticReconnect()
            .Build();

        this.hubConnection.On<MessageReceivedEvent>("ReceiveMessage", messageEvent =>
        {
            this.logger.LogInformation("GatewayClient: Received message from SignalR: {UserName}: {Text}", 
                messageEvent.Message.UserName, messageEvent.Message.Text);
            this.OnMessageReceived?.Invoke(this, messageEvent);
        });

        this.hubConnection.On<CommandReceivedEvent>("ReceiveCommand", commandEvent =>
        {
            this.logger.LogInformation("GatewayClient: Received command from SignalR: {UserName}: {CommandText}", 
                commandEvent.Command.ChatMessage.UserName, commandEvent.Command.CommandText);
            this.OnCommandReceived?.Invoke(this, commandEvent);
        });

        this.hubConnection.Closed += async error =>
        {
            if (error != null)
            {
                this.logger.LogError(error, "SignalR connection closed with error");
            }
            else
            {
                this.logger.LogInformation("SignalR connection closed");
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        };

        this.hubConnection.Reconnecting += error =>
        {
            this.logger.LogWarning(error, "SignalR connection lost, attempting to reconnect...");
            return Task.CompletedTask;
        };

        this.hubConnection.Reconnected += connectionId =>
        {
            this.logger.LogInformation("SignalR reconnected with connection ID: {ConnectionId}", connectionId);
            return Task.CompletedTask;
        };

        try
        {
            await this.hubConnection.StartAsync();
            this.logger.LogInformation("Connected to Gateway at {GatewayUrl}", gatewayUrl);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to connect to Gateway at {GatewayUrl}", gatewayUrl);
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        if (this.hubConnection != null)
        {
            await this.hubConnection.StopAsync();
            await this.hubConnection.DisposeAsync();
            this.hubConnection = null;
            this.logger.LogInformation("Disconnected from Gateway");
        }
    }

    public async Task<bool> SendMessageAsync(string message)
    {
        if (this.hubConnection == null || this.hubConnection.State != HubConnectionState.Connected)
        {
            this.logger.LogWarning("Cannot send message: not connected to Gateway");
            return false;
        }

        try
        {
            SendMessageRequest request = new() { Message = message };
            SendMessageResponse response = await this.hubConnection.InvokeAsync<SendMessageResponse>("SendMessage", request);

            if (!response.Success)
            {
                this.logger.LogWarning("Failed to send message through Gateway: {ErrorMessage}", response.ErrorMessage);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Exception while sending message through Gateway");
            return false;
        }
    }

    public void Dispose()
    {
        this.hubConnection?.DisposeAsync().AsTask().Wait();
    }
}
