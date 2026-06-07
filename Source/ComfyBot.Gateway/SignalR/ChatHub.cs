using ComfyBot.Gateway.Contracts.Events;
using ComfyBot.Gateway.Contracts.Requests;
using ComfyBot.Gateway.Contracts.Responses;
using ComfyBot.Gateway.Services;
using Microsoft.AspNetCore.SignalR;

namespace ComfyBot.Gateway.SignalR;

public class ChatHub : Hub<IChatHubClient>
{
    private readonly ILogger<ChatHub> logger;
    private readonly ITwitchService twitchService;

    public ChatHub(ILogger<ChatHub> logger, ITwitchService twitchService)
    {
        this.logger = logger;
        this.twitchService = twitchService;
    }

    public override async Task OnConnectedAsync()
    {
        this.logger.LogInformation("Client connected: {ConnectionId}", this.Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        this.logger.LogInformation("Client disconnected: {ConnectionId}", this.Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<SendMessageResponse> SendMessage(SendMessageRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return new SendMessageResponse
                {
                    Success = false,
                    ErrorMessage = "Message cannot be empty"
                };
            }

            this.twitchService.SendMessage(request.Message);

            this.logger.LogInformation("Client {ConnectionId} sent message to Twitch: {Message}", this.Context.ConnectionId, request.Message);

            return new SendMessageResponse
            {
                Success = true,
                ErrorMessage = string.Empty
            };
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to send message to Twitch from client {ConnectionId}", this.Context.ConnectionId);
            return new SendMessageResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}

public interface IChatHubClient
{
    Task ReceiveMessage(MessageReceivedEvent messageEvent);
    Task ReceiveCommand(CommandReceivedEvent commandEvent);
}
