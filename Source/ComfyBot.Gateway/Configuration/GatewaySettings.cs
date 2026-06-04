namespace ComfyBot.Gateway.Configuration;

public class GatewaySettings
{
    public const string SectionName = "Gateway";

    public string TwitchChannel { get; set; } = string.Empty;

    public string TwitchUser { get; set; } = string.Empty;

    public string TwitchAuthKey { get; set; } = string.Empty;

    public string RedisConnectionString { get; set; } = string.Empty;
}
