namespace ComfyBot.Bot.ChatBot.Commands
{
    using Wrappers;
    using Data.Models;

    public interface ITextCommandReplyLoader
    {
        bool TryGetReply(TextCommand textCommand, IChatCommand command, out string reply);
    }
}