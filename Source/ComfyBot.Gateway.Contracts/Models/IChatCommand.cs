namespace ComfyBot.Gateway.Contracts.Models;

public interface IChatCommand
{
    List<string> ArgumentsAsList { get; }

    string ArgumentsAsString { get; }

    ChatMessage ChatMessage { get; }

    string CommandText { get; }

    bool IsBroadcaster { get; }

    bool IsModerator { get; }
}
