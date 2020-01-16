namespace ComfyBot.Bot.ChatBot.Messages
{
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;

    public interface IMessageResponseLoader
    {
        bool TryGetResponse(MessageResponse messageResponse, IChatMessage message, out string response);
    }
}