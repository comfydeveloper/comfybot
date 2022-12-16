namespace ComfyBot.Bot.ChatBot.Messages;

using Wrappers;

using TwitchLib.Client.Interfaces;

public interface IMessageHandler
{
    void Handle(ITwitchClient client, IChatMessage message);
}