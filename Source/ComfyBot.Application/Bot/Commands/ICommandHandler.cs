namespace ComfyBot.Application.Bot.Commands
{
    using TwitchLib.Client.Interfaces;
    using TwitchLib.Client.Models;

    public interface ICommandHandler
    {
        void Handle(ITwitchClient client, ChatCommand command);
    }
}