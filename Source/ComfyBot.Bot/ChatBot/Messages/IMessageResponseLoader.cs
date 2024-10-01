using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IMessageResponseLoader
{
    bool TryGetResponse(MessageResponse response, IChatMessage message, out string responseText);
}