namespace ComfyBot.Bot.Gateway;

/// <summary>
/// Bridges Gateway events to Bot handler infrastructure
/// </summary>
public interface IGatewayEventBridge
{
    void RegisterHandlers();
}
