using ComfyBot.Bot.ChatBot.Wrappers;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IChatMessageHandler
{
    void Handle(IChatMessage message);
}