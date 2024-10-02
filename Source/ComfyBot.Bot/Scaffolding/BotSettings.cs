namespace ComfyBot.Bot.Scaffolding;

public class BotSettings
{
    public const string SectionName = "Bot";

    public string Channel { get; set; }

    public string AuthKey { get; set; }

    public string User { get; set; }
}