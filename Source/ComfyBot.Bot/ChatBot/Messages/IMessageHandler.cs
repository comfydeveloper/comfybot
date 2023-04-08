using ComfyBot.Bot.ChatBot.Wrappers;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IMessageHandler
{
    void Handle(ITwitchClient client, IChatMessage message);
}