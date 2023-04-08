using ComfyBot.Common.Http;

namespace ComfyBot.Bot.ChatBot.Timezones;

public class TimeLoader : ITimeLoader
{
    public TimezoneInfo GetTime(Timezone zone)
    {
        string url = BuildUrl(zone);
        return HttpService.Instance.GetAsync<TimezoneInfo>(url).Result;
    }

    private static string BuildUrl(Timezone zone)
    {
        return $"http://worldtimeapi.org/api/timezone/{zone}";
    }
}