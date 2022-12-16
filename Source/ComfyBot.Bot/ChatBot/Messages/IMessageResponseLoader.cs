namespace ComfyBot.Bot.ChatBot.Messages;

using Wrappers;
using Data.Models;

public interface IMessageResponseLoader
{
    bool TryGetResponse(MessageResponse messageResponse, IChatMessage message, out string response);
}