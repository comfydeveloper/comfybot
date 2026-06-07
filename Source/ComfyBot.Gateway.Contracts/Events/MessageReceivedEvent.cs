using ComfyBot.Gateway.Contracts.Models;

namespace ComfyBot.Gateway.Contracts.Events;

public class MessageReceivedEvent
{
    public string MessageId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public ChatMessage Message { get; set; } = new ChatMessage();
}
