using ComfyBot.Gateway.Configuration;
using ComfyBot.Gateway.Contracts.Events;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace ComfyBot.Gateway.Services;

public class RedisService : IRedisService, IDisposable
{
    private const string MessageHistoryKey = "twitch:message:history";
    private const string CommandHistoryKey = "twitch:command:history";

    private readonly ILogger<RedisService> logger;
    private readonly ConnectionMultiplexer redis;
    private readonly IDatabase database;

    public RedisService(IOptions<GatewaySettings> settings, ILogger<RedisService> logger)
    {
        this.logger = logger;
        GatewaySettings hubSettings = settings.Value;

        if (string.IsNullOrWhiteSpace(hubSettings.RedisConnectionString))
        {
            throw new InvalidOperationException("RedisConnectionString is not configured");
        }

        this.redis = ConnectionMultiplexer.Connect(hubSettings.RedisConnectionString);
        this.database = this.redis.GetDatabase();

        this.logger.LogInformation("Connected to Redis at {ConnectionString}", hubSettings.RedisConnectionString);
    }

    public async Task StoreMessageAsync(MessageReceivedEvent messageEvent)
    {
        try
        {
            string json = JsonSerializer.Serialize(messageEvent);
            double score = messageEvent.Timestamp.Ticks;
            await this.database.SortedSetAddAsync(MessageHistoryKey, json, score);
            this.logger.LogDebug("Stored message in Redis sorted set {Key}", MessageHistoryKey);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to store message in Redis");
            throw;
        }
    }

    public async Task StoreCommandAsync(CommandReceivedEvent commandEvent)
    {
        try
        {
            string json = JsonSerializer.Serialize(commandEvent);
            double score = commandEvent.Timestamp.Ticks;
            await this.database.SortedSetAddAsync(CommandHistoryKey, json, score);
            this.logger.LogDebug("Stored command in Redis sorted set {Key}", CommandHistoryKey);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to store command in Redis");
            throw;
        }
    }

    public async Task<List<MessageReceivedEvent>> GetMessagesAsync(int count, long offset)
    {
        try
        {
            RedisValue[] values = await this.database.SortedSetRangeByScoreAsync(
                MessageHistoryKey,
                start: offset,
                stop: double.PositiveInfinity,
                order: Order.Ascending,
                take: count);

            List<MessageReceivedEvent> messages = [];
            foreach (RedisValue value in values)
            {
                if (value.HasValue)
                {
                    MessageReceivedEvent? messageEvent = JsonSerializer.Deserialize<MessageReceivedEvent>(value.ToString());
                    if (messageEvent != null)
                    {
                        messages.Add(messageEvent);
                    }
                }
            }

            this.logger.LogDebug("Retrieved {Count} messages from Redis", messages.Count);
            return messages;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to retrieve messages from Redis");
            throw;
        }
    }

    public void Dispose()
    {
        this.redis?.Dispose();
    }
}
