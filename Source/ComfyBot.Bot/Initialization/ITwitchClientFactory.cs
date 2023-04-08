using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Initialization;

public interface ITwitchClientFactory
{
    ITwitchClient Create();
}