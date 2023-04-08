using System;
using System.Text.Json.Serialization;

namespace ComfyBot.Bot.ChatBot.Timezones;

public class TimezoneInfo
{
    [JsonPropertyName("datetime")]
    public DateTimeOffset DateTime { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; }
}