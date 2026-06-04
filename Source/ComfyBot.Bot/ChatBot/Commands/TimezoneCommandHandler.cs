using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Bot.Extensions;

namespace ComfyBot.Bot.ChatBot.Commands;

public class TimezoneCommandHandler : CommandHandler
{
    private readonly ITimezoneLoader zoneLoader;
    private readonly ITimeLoader timeLoader;
    private readonly IMessageSender messageSender;

    public TimezoneCommandHandler(ITimezoneLoader zoneLoader, ITimeLoader timeLoader, IMessageSender messageSender)
    {
        this.zoneLoader = zoneLoader;
        this.timeLoader = timeLoader;
        this.messageSender = messageSender;
    }

    protected override bool CanHandle(IChatCommand command)
    {
        return command.Is("timezone") && command.HasParameters();
    }

    protected override void HandleInternal(IChatCommand chatCommand)
    {
        if (this.zoneLoader.TryLoad(chatCommand.ArgumentsAsString, out Timezone timezone))
        {
            TimezoneInfo timezoneInfo = this.timeLoader.GetTime(timezone);

            this.messageSender.Send($"{chatCommand.ChatMessage.UserName}: {timezoneInfo.Timezone} {timezoneInfo.DateTime:G}");
        }
        else
        {
            this.messageSender.Send($"Sorry {chatCommand.ChatMessage.UserName}, can't find timezone info for '{chatCommand.ArgumentsAsString}'.");
        }
    }
}