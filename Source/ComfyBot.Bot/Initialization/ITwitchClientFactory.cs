namespace ComfyBot.Bot.Initialization
{
    using TwitchLib.Client.Interfaces;

    public interface ITwitchClientFactory
    {
        ITwitchClient Create(string userName, string password, string channel);
    }
}