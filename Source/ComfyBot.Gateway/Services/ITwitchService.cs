using ComfyBot.Gateway.Contracts.Events;

namespace ComfyBot.Gateway.Services;

public interface ITwitchService
{
    event EventHandler<MessageReceivedEvent>? OnMessageReceived;
    event EventHandler<CommandReceivedEvent>? OnCommandReceived;

    void Connect();
    void Disconnect();
    void SendMessage(string message);
}
