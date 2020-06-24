namespace ComfyBot.Bot.ChatBot.Commands
{
    using System.Linq;

    using ComfyBot.Bot.ChatBot.Weather;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Bot.Extensions;
    using ComfyBot.Common.Http;
    using ComfyBot.Settings;

    using TwitchLib.Client.Interfaces;

    public class WeatherCommandHandler : CommandHandler
    {
        protected override bool CanHandle(IChatCommand command)
        {
            return (command.Is("weather") || command.Is("wetter")) && command.HasParameters();
        }

        protected override void HandleInternal(ITwitchClient client, IChatCommand command)
        {
            CityWeather result = HttpService.Instance.GetAsync<CityWeather>($"https://api.openweathermap.org/data/2.5/weather?q={command.ArgumentsAsString}&appid={Settings.ApplicationSettings.Default.OpenWeatherMapApiKey}&lang={Settings.ApplicationSettings.Default.OpenWeatherMapApiLang}&units=metric").Result;

            if (ApplicationSettings.Default.OpenWeatherMapApiLang == "DE")
            {
                this.HandleGerman(client, command, result);
            }
            else
            {
                this.HandleEnglish(client, command, result);
            }
        }

        private void HandleEnglish(ITwitchClient client, IChatCommand command, CityWeather result)
        {
            if (result.City == null)
            {
                this.SendMessage(client, $"Sorry {command.ChatMessage.UserName}, I couldn't find any weather data for {command.ArgumentsAsString}");
            }
            else
            {
                this.SendMessage(client, $"The weather in {result.City} is {result.Weather.First().Description}. It is {result.Temperatures.Temperature:0.#}°C ({result.Temperatures.FeelsLike:0.#}°C) with {result.Temperatures.Humidity}% humidity.");
            }
        }

        private void HandleGerman(ITwitchClient client, IChatCommand command, CityWeather result)
        {
            if (result.City == null)
            {
                this.SendMessage(client, $"Tut mir leid {command.ChatMessage.UserName}, ich konnte keine Wetterdaten zu {command.ArgumentsAsString} finden.");
            }
            else
            {
                this.SendMessage(client, $"Wetter in {result.City}: {result.Weather.First().Description}. Es sind gerade {result.Temperatures.Temperature:0.#}°C ({result.Temperatures.FeelsLike:0.#}°C) mit {result.Temperatures.Humidity}% Luftfeuchtigkeit.");
            }
        }
    }
}