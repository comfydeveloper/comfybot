namespace ComfyBot.Bot.ChatBot.Commands
{
    using Wrappers;
    using Settings;

    using TwitchLib.Client.Interfaces;

    public abstract class CommandHandler : ICommandHandler
    {
        public void Handle(ITwitchClient client, IChatCommand command)
        {
            if (CanHandle(command))
            {
                HandleInternal(client, command);
            }
        }

        protected abstract bool CanHandle(IChatCommand command);

        protected abstract void HandleInternal(ITwitchClient client, IChatCommand command);

        protected void SendMessage(ITwitchClient client, string message)
        {
            client.SendMessage(ApplicationSettings.Default.Channel, message);
        }
    }
}