namespace ComfyBot.Bot.ChatBot.Messages
{
    using ComfyBot.Bot.ChatBot.Chatters;
    using ComfyBot.Bot.ChatBot.Wrappers;

    using TwitchLib.Client.Interfaces;

    public class ActivityUpdater : IMessageHandler
    {
        private readonly IChattersCache cache;

        public ActivityUpdater(IChattersCache cache)
        {
            this.cache = cache;
        }

        public void Handle(ITwitchClient client, IChatMessage message)
        {
            this.cache.UpdateActivity(message.UserName);
        }
    }
}