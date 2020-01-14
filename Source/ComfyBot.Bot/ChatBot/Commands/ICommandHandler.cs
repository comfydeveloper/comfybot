namespace ComfyBot.Bot.ChatBot.Commands
{
    using global::ComfyBot.Bot.ChatBot.Wrappers;

    using TwitchLib.Client.Interfaces;

    public interface ICommandHandler
    {
        void Handle(ITwitchClient client, IChatCommand command);
    }
}