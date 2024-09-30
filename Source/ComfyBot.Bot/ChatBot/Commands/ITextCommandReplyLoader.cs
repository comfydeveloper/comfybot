using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;

namespace ComfyBot.Bot.ChatBot.Commands;

public interface ITextCommandReplyLoader
{
    bool TryGetReply(TextCommandOld textCommandOld, IChatCommand command, out string reply);
}