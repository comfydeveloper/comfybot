namespace ComfyBot.Bot.ChatBot.Weather
{
    using System.Text.Json.Serialization;

    public class TemperatureDetail
    {
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }

        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }
    }
}