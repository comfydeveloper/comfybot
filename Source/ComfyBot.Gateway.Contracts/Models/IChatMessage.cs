namespace ComfyBot.Gateway.Contracts.Models;

public interface IChatMessage
{
    bool IsBroadcaster { get; }

    bool IsModerator { get; }

    string UserName { get; }

    string Text { get; }
}
