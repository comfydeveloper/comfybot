namespace ComfyBot.Bot.ChatBot.Commands
{
    using global::ComfyBot.Bot.ChatBot.Wrappers;
    using global::ComfyBot.Data.Models;
    using global::ComfyBot.Data.Repositories;

    using TwitchLib.Client.Interfaces;

    public class ShoutoutCommandHandler : CommandHandler
    {
        private readonly IRepository<Shoutout> repository;

        public ShoutoutCommandHandler(IRepository<Shoutout> repository)
        {
            this.repository = repository;
        }

        protected override bool CanHandle(IChatCommand command)
        {
            return command.CommandText.ToLower().StartsWith("so");
        }

        protected override void HandleInternal(ITwitchClient client, IChatCommand command)
        {
            Shoutout model = this.repository.Get(command.ArgumentsAsList[0]);

            if (model != null)
            {
                this.SendMessage(client, $@"▬▬☆✼★▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬★✼☆▬▬
                                                {model.Text}      
                                                ▬▬☆✼★━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━★✼☆▬▬");
            }
        }
    }
}