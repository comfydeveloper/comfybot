using System.Threading.Tasks;
using ComfyBot.Gateway.Contracts.Models;

namespace ComfyBot.Bot.ChatBot.Commands;

public abstract class CommandHandler : ICommandHandler
{
    public async Task Handle(IChatCommand command)
    {
        if (this.CanHandle(command))
        {
            await this.HandleInternal(command);
        }
    }

    protected abstract bool CanHandle(IChatCommand command);

    protected abstract Task HandleInternal(IChatCommand chatCommand);
}