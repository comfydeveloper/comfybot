using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Commands;

public abstract class CommandHandler : ICommandHandler
{
    private readonly BotSettings settings;

    protected CommandHandler(IOptions<BotSettings> settings)
    {
        this.settings = settings.Value;
    }

    public void Handle(ITwitchClient client, IChatCommand command)
    {
        if (this.CanHandle(command))
        {
            this.HandleInternal(client, command);
        }
    }

    protected abstract bool CanHandle(IChatCommand command);

    protected abstract void HandleInternal(ITwitchClient client, IChatCommand chatCommand);

    protected void SendMessage(ITwitchClient client, string message)
    {
        client.SendMessage(this.settings.Channel, message);
    }
}