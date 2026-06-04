using ComfyBot.Gateway.Contracts.Models;

namespace ComfyBot.Gateway.Contracts.Events;

public class CommandReceivedEvent
{
    public string MessageId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public ChatCommand Command { get; set; } = new ChatCommand();
}
