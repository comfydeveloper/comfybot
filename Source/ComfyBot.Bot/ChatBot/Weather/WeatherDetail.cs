namespace ComfyBot.Bot.ChatBot.Weather
{
    using System.Text.Json.Serialization;

    public class WeatherDetail
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}