namespace ComfyBot.Application.Bot
{
    using global::ComfyBot.Application.Bot.Initialization;

    using Microsoft.Extensions.Configuration;

    using TwitchLib.Client.Interfaces;

    public class ComfyBot : IComfyBot
    {
        private readonly IConfiguration configuration;
        private readonly ITwitchClientFactory twitchClientFactory;

        private ITwitchClient twitchClient;

        public ComfyBot(IConfiguration configuration,
                        ITwitchClientFactory twitchClientFactory)
        {
            this.configuration = configuration;
            this.twitchClientFactory = twitchClientFactory;
        }

        public void Run()
        {
            this.InitializeClient();
        }

        private void InitializeClient()
        {
            this.twitchClient = this.twitchClientFactory.Create(this.configuration["user"],
                                                                this.configuration["password"]);
            this.twitchClient.JoinChannel(this.configuration["channel"]);
            this.twitchClient.Connect();
        }
    }
}