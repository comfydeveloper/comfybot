using ComfyBot.Gateway.Contracts.Models;

namespace ComfyBot.Bot.ChatBot.Commands;

public interface ICommandHandler
{
    void Handle(IChatCommand command);
}