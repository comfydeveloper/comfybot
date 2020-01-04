namespace ComfyBot.Application.Bot.Commands
{
    using global::ComfyBot.Application.Bot.Wrappers;

    using TwitchLib.Client.Interfaces;

    public interface ICommandHandler
    {
        void Handle(ITwitchClient client, IChatCommand command);
    }
}