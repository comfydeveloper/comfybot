namespace ComfyBot.Bot.Scaffolding;

public class BotSettings
{
    public const string SectionName = "Bot";

    public string Channel { get; set; }

    public string AuthKey { get; set; }

    public string User { get; set; }

    /// <summary>
    /// Gets or sets a list of users that should be ignored by the bot when responding to chat messages.
    /// </summary>
    public string[] IgnoredUsers { get; set; } = [];

    /// <summary>
    /// Gets or sets the Gateway service URL. If null, uses direct TwitchLib connection (legacy mode).
    /// </summary>
    public string GatewayUrl { get; set; }
}
