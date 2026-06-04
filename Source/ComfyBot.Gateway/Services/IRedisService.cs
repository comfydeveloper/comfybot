using ComfyBot.Gateway.Contracts.Events;

namespace ComfyBot.Gateway.Services;

public interface IRedisService
{
    Task StoreMessageAsync(MessageReceivedEvent messageEvent);
    Task StoreCommandAsync(CommandReceivedEvent commandEvent);
    Task<List<MessageReceivedEvent>> GetMessagesAsync(int count, long offset);
}
