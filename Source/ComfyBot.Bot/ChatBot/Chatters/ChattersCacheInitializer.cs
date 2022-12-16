namespace ComfyBot.Bot.ChatBot.Chatters
{
    using Common.Http;
    using ComfyBot.Common.Initialization;

    public class ChattersCacheInitializer : IInitializerJob
    {
        private readonly IChattersCache chattersCache;

        public ChattersCacheInitializer(IChattersCache chattersCache)
        {
            this.chattersCache = chattersCache;
        }

        public void Execute()
        {
            if(string.IsNullOrEmpty(Settings.ApplicationSettings.Default.Channel))
            {
                return; 
            }

            ChattersCollection result = HttpService.Instance.GetAsync<ChattersCollection>($"https://tmi.twitch.tv/group/user/{Settings.ApplicationSettings.Default.Channel.ToLower()}/chatters").Result;
            chattersCache.AddRange(result.Chatters.All());
        }
    }
}