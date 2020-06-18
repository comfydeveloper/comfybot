namespace ComfyBot.Bot.ChatBot.Commands
{
    using ComfyBot.Bot.ChatBot.Timezones;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Bot.Extensions;

    using TwitchLib.Client.Interfaces;

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
            if (this.zoneLoader.TryLoad(command.ArgumentsAsList[0], out Timezone timezone))
            {
                TimezoneInfo timezoneInfo = this.timeLoader.GetTime(timezone);

                this.SendMessage(client, $"{command.ChatMessage.UserName}: {timezoneInfo.Timezone} {timezoneInfo.DateTime:G}");
            }
            else
            {
                this.SendMessage(client, $"Sorry {command.ChatMessage.UserName}, can't find timezone info for '{command.ArgumentsAsList[0]}'.");
            }
        }
    }
}