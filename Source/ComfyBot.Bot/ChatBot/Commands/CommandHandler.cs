using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Settings;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Commands;

public abstract class CommandHandler : ICommandHandler
{
    public void Handle(ITwitchClient client, IChatCommand command)
    {
        if (CanHandle(command))
        {
            HandleInternal(client, command);
        }
    }

    protected abstract bool CanHandle(IChatCommand command);

    protected abstract void HandleInternal(ITwitchClient client, IChatCommand command);

    protected void SendMessage(ITwitchClient client, string message)
    {
        client.SendMessage(ApplicationSettings.Default.Channel, message);
    }
}