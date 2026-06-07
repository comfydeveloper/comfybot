using ComfyBot.Bot.Gateway;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Options;

namespace ComfyBot.Bot.ChatBot.Services;

public class MessageSender : IMessageSender
{
    private readonly IGatewayClient gatewayClient;
    private readonly BotSettings settings;

    public MessageSender(IGatewayClient gatewayClient, IOptions<BotSettings> settings)
    {
        this.gatewayClient = gatewayClient;
        this.settings = settings.Value;
    }

    public void Send(string message)
    {
        this.gatewayClient.SendMessageAsync(message).GetAwaiter().GetResult();
    }
}