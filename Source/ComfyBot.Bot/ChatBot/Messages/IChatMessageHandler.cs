using ComfyBot.Bot.ChatBot.Wrappers;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IChatMessageHandler
{
    void Handle(ITwitchClient client, IChatMessage message);
}