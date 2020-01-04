namespace ComfyBot.Application.Bot.Wrappers
{
    public interface IChatMessage
    {
        bool IsBroadcaster { get; }

        bool IsModerator { get; }
    }
}