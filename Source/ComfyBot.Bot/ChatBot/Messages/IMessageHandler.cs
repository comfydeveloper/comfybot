namespace ComfyBot.Bot.ChatBot.Messages
{
    using ComfyBot.Bot.ChatBot.Wrappers;

    using TwitchLib.Client.Interfaces;

    public interface IMessageHandler
    {
        void Handle(ITwitchClient client, IChatMessage message);
    }
}