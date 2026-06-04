namespace ComfyBot.Gateway.Contracts.Models;

public class ChatCommand : IChatCommand
{
    public DateTime Timestamp { get; set; }

    public List<string> ArgumentsAsList { get; set; } = [];

    public string ArgumentsAsString { get; set; } = string.Empty;

    public IChatMessage ChatMessage { get; set; } = new ChatMessage();

    public string CommandText { get; set; } = string.Empty;

    public bool IsBroadcaster { get; set; }

    public bool IsModerator { get; set; }
}
