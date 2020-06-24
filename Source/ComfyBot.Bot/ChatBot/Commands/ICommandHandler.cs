namespace ComfyBot.Bot.ChatBot.Commands
{
    using global::ComfyBot.Bot.ChatBot.Wrappers;

    using TwitchLib.Client.Interfaces;

    public interface ICommandHandler
    {
        //TODO Remove the client from this into its own service that can be injected & called
        void Handle(ITwitchClient client, IChatCommand command);
    }
}