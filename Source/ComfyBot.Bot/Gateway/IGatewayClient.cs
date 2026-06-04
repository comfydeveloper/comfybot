using System;
using System.Threading.Tasks;
using ComfyBot.Gateway.Contracts.Events;

namespace ComfyBot.Bot.Gateway;

public interface IGatewayClient
{
    event EventHandler<MessageReceivedEvent>? OnMessageReceived;
    event EventHandler<CommandReceivedEvent>? OnCommandReceived;

    Task ConnectAsync();
    Task DisconnectAsync();
    Task<bool> SendMessageAsync(string message);
}
