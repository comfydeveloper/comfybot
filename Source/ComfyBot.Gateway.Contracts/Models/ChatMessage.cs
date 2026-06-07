namespace ComfyBot.Gateway.Contracts.Models;

public class ChatMessage : IChatMessage
{
    public DateTime Timestamp { get; set; }

    public bool IsBroadcaster { get; set; }

    public bool IsModerator { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}
