using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Extensions;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Commands;

public class TimezoneCommandHandler : CommandHandler
{
    private readonly ITimezoneLoader zoneLoader;
    private readonly ITimeLoader timeLoader;

    public TimezoneCommandHandler(ITimezoneLoader zoneLoader, ITimeLoader timeLoader, IOptions<BotSettings> settings) : base(settings)
    {
        this.zoneLoader = zoneLoader;
        this.timeLoader = timeLoader;
    }

    protected override bool CanHandle(IChatCommand command)
    {
        return command.Is("timezone") && command.HasParameters();
    }

    protected override void HandleInternal(ITwitchClient client, IChatCommand chatCommand)
    {
        if (this.zoneLoader.TryLoad(chatCommand.ArgumentsAsString, out Timezone timezone))
        {
            TimezoneInfo timezoneInfo = this.timeLoader.GetTime(timezone);

            this.SendMessage(client, $"{chatCommand.ChatMessage.UserName}: {timezoneInfo.Timezone} {timezoneInfo.DateTime:G}");
        }
        else
        {
            this.SendMessage(client, $"Sorry {chatCommand.ChatMessage.UserName}, can't find timezone info for '{chatCommand.ArgumentsAsString}'.");
        }
    }
}