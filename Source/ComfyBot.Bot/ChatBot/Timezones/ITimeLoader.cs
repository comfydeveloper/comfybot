namespace ComfyBot.Bot.ChatBot.Timezones;

public interface ITimeLoader
{
    TimezoneInfo GetTime(Timezone zone);
}