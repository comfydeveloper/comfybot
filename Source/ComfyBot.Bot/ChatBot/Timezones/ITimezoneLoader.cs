namespace ComfyBot.Bot.ChatBot.Timezones
{
    public interface ITimezoneLoader
    {
        bool TryLoad(string zone, out Timezone result);
    }
}