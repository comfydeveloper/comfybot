namespace ComfyBot.Bot.ChatBot.Commands
{
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;

    public interface ITextCommandReplyLoader
    {
        bool TryGetReply(TextCommand textCommand, IChatCommand command, out string reply);
    }
}