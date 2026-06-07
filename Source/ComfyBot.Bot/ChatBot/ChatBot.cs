using System;
using Microsoft.Extensions.Logging;
using ComfyBot.Bot.Gateway;
using System.Threading.Tasks;

namespace ComfyBot.Bot.ChatBot;

public class ChatBot : IComfyBot
{
    private readonly ILogger<ChatBot> logger;
    private readonly IGatewayClient gatewayClient;
    private readonly IGatewayEventBridge eventBridge;

    public ChatBot(
        IGatewayClient gatewayClient,
        IGatewayEventBridge eventBridge,
        ILogger<ChatBot> logger)
    {
        this.gatewayClient = gatewayClient;
        this.eventBridge = eventBridge;
        this.logger = logger;
    }

    public async Task Run()
    {
        try
        {
            this.eventBridge.RegisterHandlers();
            await this.gatewayClient.ConnectAsync();
            Log("Bot initialized and connected to Gateway.");
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ex, "Failed to initialize bot");
            throw;
        }
    }

    private static void Log(string message)
    {
        Console.Write($"{DateTime.Now}: {message}");
    }
}
