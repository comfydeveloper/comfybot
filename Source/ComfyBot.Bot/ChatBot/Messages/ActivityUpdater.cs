namespace ComfyBot.Bot.ChatBot.Messages
{
    using Chatters;
    using Wrappers;

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
            cache.UpdateActivity(message.UserName);
        }
    }
}