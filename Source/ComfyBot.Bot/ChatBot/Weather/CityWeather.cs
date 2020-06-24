namespace ComfyBot.Bot.ChatBot.Weather
{
    using System.Text.Json.Serialization;

    public class CityWeather
    {
        [JsonPropertyName("name")]
        public string City { get; set; }

        [JsonPropertyName("weather")]
        public WeatherDetail[] Weather { get; set; }

        [JsonPropertyName("main")]
        public TemperatureDetail Temperatures { get; set; }
    }
}