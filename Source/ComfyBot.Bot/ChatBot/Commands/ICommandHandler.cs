using ComfyBot.Bot.ChatBot.Wrappers;

namespace ComfyBot.Bot.ChatBot.Commands;

public interface ICommandHandler
{
    void Handle(IChatCommand command);
}