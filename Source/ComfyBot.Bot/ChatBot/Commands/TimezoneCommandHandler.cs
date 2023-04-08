using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Extensions;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Commands;

public class TimezoneCommandHandler : CommandHandler
{
    private readonly ITimezoneLoader zoneLoader;
    private readonly ITimeLoader timeLoader;

    public TimezoneCommandHandler(ITimezoneLoader zoneLoader, ITimeLoader timeLoader)
    {
        this.zoneLoader = zoneLoader;
        this.timeLoader = timeLoader;
    }

    protected override bool CanHandle(IChatCommand command)
    {
        return command.Is("timezone") && command.HasParameters();
    }

    protected override void HandleInternal(ITwitchClient client, IChatCommand command)
    {
        if (zoneLoader.TryLoad(command.ArgumentsAsString, out Timezone timezone))
        {
            TimezoneInfo timezoneInfo = timeLoader.GetTime(timezone);

            SendMessage(client, $"{command.ChatMessage.UserName}: {timezoneInfo.Timezone} {timezoneInfo.DateTime:G}");
        }
        else
        {
            SendMessage(client, $"Sorry {command.ChatMessage.UserName}, can't find timezone info for '{command.ArgumentsAsString}'.");
        }
    }
}