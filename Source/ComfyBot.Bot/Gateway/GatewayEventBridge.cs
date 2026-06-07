using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Gateway.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ComfyBot.Bot.Gateway;

/// <summary>
/// Bridges Gateway events to Bot handler infrastructure
/// </summary>
public class GatewayEventBridge : IGatewayEventBridge
{
    private readonly ILogger<GatewayEventBridge> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IGatewayClient gatewayClient;

    public GatewayEventBridge(
        IGatewayClient gatewayClient,
        ILogger<GatewayEventBridge> logger,
        IServiceProvider serviceProvider)
    {
        this.gatewayClient = gatewayClient;
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    public void RegisterHandlers()
    {
        this.logger.LogInformation("GatewayEventBridge: Registering event handlers");
        this.gatewayClient.OnMessageReceived += this.OnMessageReceived;
        this.gatewayClient.OnCommandReceived += this.OnCommandReceived;
        this.logger.LogInformation("GatewayEventBridge: Event handlers registered");
    }

    private void OnMessageReceived(object sender, MessageReceivedEvent messageEvent)
    {
        // Fire and forget pattern - don't block SignalR event
        _ = this.HandleMessageAsync(messageEvent);
    }

    private async Task HandleMessageAsync(MessageReceivedEvent messageEvent)
    {
        this.logger.LogInformation("GatewayEventBridge: Received message event from Gateway. User: {User}, Text: {Text}",
            messageEvent.Message.UserName, messageEvent.Message.Text);

        // Create scope that will live for the duration of async operations
        using IServiceScope serviceScope = this.serviceProvider.CreateScope();
        IEnumerable<IChatMessageHandler> messageHandlers = serviceScope.ServiceProvider.GetServices<IChatMessageHandler>();

        this.logger.LogDebug("GatewayEventBridge: Found {Count} message handlers", messageHandlers.Count());

        foreach (IChatMessageHandler handler in messageHandlers)
        {
            try
            {
                this.logger.LogDebug("GatewayEventBridge: Invoking handler {HandlerType}", handler.GetType().Name);
                await handler.Handle(messageEvent.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to handle message {@Message}", messageEvent.Message.Text);
            }
        }
    }

    private void OnCommandReceived(object sender, CommandReceivedEvent commandEvent)
    {
        // Fire and forget pattern - don't block SignalR event
        _ = this.HandleCommandAsync(commandEvent);
    }

    private async Task HandleCommandAsync(CommandReceivedEvent commandEvent)
    {
        this.logger.LogInformation("GatewayEventBridge: Received command event from Gateway. User: {User}, Command: {Command}",
            commandEvent.Command.ChatMessage.UserName, commandEvent.Command.CommandText);

        using IServiceScope serviceScope = this.serviceProvider.CreateScope();
        IEnumerable<ICommandHandler> commandHandlers = serviceScope.ServiceProvider.GetServices<ICommandHandler>();

        this.logger.LogDebug("GatewayEventBridge: Found {Count} command handlers", commandHandlers.Count());

        foreach (ICommandHandler handler in commandHandlers)
        {
            try
            {
                this.logger.LogDebug("GatewayEventBridge: Invoking handler {HandlerType}", handler.GetType().Name);
                await handler.Handle(commandEvent.Command);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to handle command {@CommandText}", commandEvent.Command.CommandText);
            }
        }
    }
}
