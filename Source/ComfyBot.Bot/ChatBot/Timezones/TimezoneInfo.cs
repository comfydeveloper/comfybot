namespace ComfyBot.Bot.ChatBot.Timezones;

using System;
using System.Text.Json.Serialization;

public class TimezoneInfo
{
    [JsonPropertyName("datetime")]
    public DateTimeOffset DateTime { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }
}