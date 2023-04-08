using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;

namespace ComfyBot.Bot.ChatBot.Commands;

public interface ITextCommandReplyLoader
{
    bool TryGetReply(TextCommand textCommand, IChatCommand command, out string reply);
}