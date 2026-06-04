using ComfyBot.Gateway.Configuration;
using ComfyBot.Gateway.Contracts.Events;
using Microsoft.Extensions.Options;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using InternalChatMessage = ComfyBot.Gateway.Contracts.Models.ChatMessage;
using InternalChatCommand = ComfyBot.Gateway.Contracts.Models.ChatCommand;

namespace ComfyBot.Gateway.Services;

public class TwitchService : ITwitchService
{
    private readonly ILogger<TwitchService> logger;
    private readonly GatewaySettings settings;
    private readonly ITwitchClient twitchClient;

    public event EventHandler<MessageReceivedEvent>? OnMessageReceived;
    public event EventHandler<CommandReceivedEvent>? OnCommandReceived;

    public TwitchService(IOptions<GatewaySettings> settings, ILogger<TwitchService> logger)
    {
        this.logger = logger;
        this.settings = settings.Value;

        ConnectionCredentials credentials = new(this.settings.TwitchUser, this.settings.TwitchAuthKey);
        ClientOptions clientOptions = new() 
        { 
            MessagesAllowedInPeriod = 100, 
            ThrottlingPeriod = TimeSpan.FromSeconds(30) 
        };
        WebSocketClient websocketClient = new(clientOptions);
        this.twitchClient = new TwitchClient(websocketClient);
        this.twitchClient.Initialize(credentials, this.settings.TwitchChannel);

        this.RegisterHandlers();
    }

    public void Connect()
    {
        this.ValidateSettings();
        this.twitchClient.Connect();
        this.logger.LogInformation("Connecting to Twitch channel {Channel}", this.settings.TwitchChannel);
    }

    public void Disconnect()
    {
        this.twitchClient.Disconnect();
        this.logger.LogInformation("Disconnected from Twitch");
    }

    public void SendMessage(string message)
    {
        this.twitchClient.SendMessage(this.settings.TwitchChannel, message);
        this.logger.LogDebug("Sent message to Twitch: {Message}", message);
    }

    private void RegisterHandlers()
    {
        this.twitchClient.OnMessageReceived += this.HandleMessageReceived;
        this.twitchClient.OnChatCommandReceived += this.HandleCommandReceived;
        this.twitchClient.OnConnected += this.HandleConnected;
        this.twitchClient.OnJoinedChannel += this.HandleJoinedChannel;
        this.twitchClient.OnLog += this.HandleLog;
    }

    private void HandleMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        try
        {
            // Skip command messages - they will be handled by HandleCommandReceived
            if (e.ChatMessage.Message.StartsWith('!'))
            {
                return;
            }

            DateTime timestamp = DateTime.UtcNow;
            MessageReceivedEvent messageEvent = new()
            {
                MessageId = Guid.CreateVersion7().ToString(),
                Timestamp = timestamp,
                Message = new InternalChatMessage
                {
                    Timestamp = timestamp,
                    IsBroadcaster = e.ChatMessage.IsBroadcaster,
                    IsModerator = e.ChatMessage.IsModerator,
                    UserName = e.ChatMessage.Username,
                    Text = e.ChatMessage.Message
                }
            };

            this.OnMessageReceived?.Invoke(this, messageEvent);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to handle message from {User}", e.ChatMessage.Username);
        }
    }

    private void HandleCommandReceived(object? sender, OnChatCommandReceivedArgs e)
    {
        try
        {
            DateTime timestamp = DateTime.UtcNow;
            CommandReceivedEvent commandEvent = new()
            {
                MessageId = Guid.CreateVersion7().ToString(),
                Timestamp = timestamp,
                Command = new InternalChatCommand
                {
                    Timestamp = timestamp,
                    CommandText = e.Command.CommandText,
                    ArgumentsAsList = [.. e.Command.ArgumentsAsList],
                    ArgumentsAsString = e.Command.ArgumentsAsString,
                    IsBroadcaster = e.Command.ChatMessage.IsBroadcaster,
                    IsModerator = e.Command.ChatMessage.IsModerator,
                    ChatMessage = new InternalChatMessage
                    {
                        Timestamp = timestamp,
                        IsBroadcaster = e.Command.ChatMessage.IsBroadcaster,
                        IsModerator = e.Command.ChatMessage.IsModerator,
                        UserName = e.Command.ChatMessage.Username,
                        Text = e.Command.ChatMessage.Message
                    }
                }
            };

            this.OnCommandReceived?.Invoke(this, commandEvent);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to handle command {Command} from {User}", 
                e.Command.CommandText, e.Command.ChatMessage.Username);
        }
    }

    private void HandleConnected(object? sender, OnConnectedArgs e)
    {
        this.logger.LogInformation("Successfully connected to Twitch");
    }

    private void HandleJoinedChannel(object? sender, OnJoinedChannelArgs e)
    {
        this.logger.LogInformation("Successfully joined channel {Channel}", e.Channel);
    }

    private void HandleLog(object? sender, OnLogArgs e)
    {
        this.logger.LogDebug("TwitchLib: {BotUsername} - {Data}", e.BotUsername, e.Data);
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(this.settings.TwitchChannel))
        {
            throw new InvalidOperationException("TwitchChannel is not configured");
        }

        if (string.IsNullOrWhiteSpace(this.settings.TwitchAuthKey))
        {
            throw new InvalidOperationException("TwitchAuthKey is not configured");
        }

        if (string.IsNullOrWhiteSpace(this.settings.TwitchUser))
        {
            throw new InvalidOperationException("TwitchUser is not configured");
        }
    }
}
