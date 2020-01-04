namespace ComfyBot.Application.Bot.Commands
{
    using global::ComfyBot.Application.Bot.Wrappers;

    using Microsoft.Extensions.Configuration;

    using TwitchLib.Client.Interfaces;

    public abstract class CommandHandler : ICommandHandler
    {
        private readonly IConfiguration configuration;

        protected CommandHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Handle(ITwitchClient client, IChatCommand command)
        {
            if (this.CanHandle(command))
            {
                this.HandleInternal(client, command);
            }
        }

        protected abstract bool CanHandle(IChatCommand command);

        protected abstract void HandleInternal(ITwitchClient client, IChatCommand command);

        protected void SendMessage(ITwitchClient client, string message)
        {
            client.SendMessage(this.configuration["channel"], message);
        }
    }
}