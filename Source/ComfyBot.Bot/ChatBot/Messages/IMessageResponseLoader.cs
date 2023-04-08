using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IMessageResponseLoader
{
    bool TryGetResponse(MessageResponse messageResponse, IChatMessage message, out string response);
}