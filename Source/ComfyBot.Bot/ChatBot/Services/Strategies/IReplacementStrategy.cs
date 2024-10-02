namespace ComfyBot.Bot.ChatBot.Services.Strategies;

public interface IReplacementStrategy
{
    public string Replace(string text, WildcardReplacerOptions options);
}