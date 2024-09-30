using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IMessageResponseLoader
{
    bool TryGetResponse(MessageResponseOld messageResponseOld, IChatMessage message, out string response);
}