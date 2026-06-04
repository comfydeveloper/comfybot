using ComfyBot.Gateway.Contracts.Models;

namespace ComfyBot.Bot.ChatBot.Commands;

public abstract class CommandHandler : ICommandHandler
{
    public void Handle(IChatCommand command)
    {
        if (this.CanHandle(command))
        {
            this.HandleInternal(command);
        }
    }

    protected abstract bool CanHandle(IChatCommand command);

    protected abstract void HandleInternal(IChatCommand chatCommand);
}