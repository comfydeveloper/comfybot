# ComfyBot - Gateway Architecture Setup

The bot uses a Gateway service architecture. The Gateway handles Twitch connectivity, while the Bot processes messages/commands through SignalR.

## Architecture Overview

```
Twitch Chat → Gateway Service (SignalR Hub) → Bot Application → Command/Message Handlers
					 ↓
				Redis (persistence)
```

## Running the System

### 1. Start Redis

```bash
docker run -d -p 6379:6379 redis:latest
```

Or use an existing Redis instance and configure the connection string.

### 2. Configure Gateway

Create `.env` file in `Source\ComfyBot.Gateway\bin\Debug\net10.0\`:

```bash
# Copy from .env.example
Gateway__TwitchChannel=yourchannelname
Gateway__TwitchUser=yourbotusername
Gateway__TwitchAuthKey=oauth:your_token_here
Gateway__RedisConnectionString=localhost:6379
```

Get your Twitch OAuth token from: https://twitchapps.com/tmi/

### 3. Start Gateway Service

```bash
cd Source\ComfyBot.Gateway
dotnet run
```

The Gateway will:
- Connect to Twitch chat
- Start SignalR hub on `http://localhost:5125/chat`
- Store messages in Redis
- Broadcast messages to connected clients

### 4. Configure Bot Application

The Bot is configured in `Source\ComfyBot.Application\appsettings.json`:

```json
{
  "Bot": {
	"IgnoredUsers": ["Streamelements"],
	"GatewayUrl": "http://localhost:5125"
  }
}
```

### 5. Start Bot Application

Run the ComfyBot.Application project from Visual Studio or:

```bash
cd Source\ComfyBot.Application
dotnet run
```

The Bot will:
- Connect to the Gateway via SignalR
- Subscribe to message/command events
- Process messages through existing handlers

## Migration Notes

### What Changed

- **Old**: Bot connected directly to Twitch using TwitchLib
- **New**: Gateway connects to Twitch; Bot connects to Gateway via SignalR

### Bot Wrappers Removed

The old `IChatCommand` and `IChatMessage` wrapper interfaces in `ComfyBot.Bot.ChatBot.Wrappers` have been replaced with `ComfyBot.Gateway.Contracts.Models` interfaces. All handlers now use the Gateway contracts directly.

### Sending Messages

Messages are now sent through the Gateway's SignalR hub:

```csharp
await gatewayClient.SendMessageAsync("Hello chat!");
```

The Gateway forwards the message to Twitch.

## Configuration Options

### BotSettings

- `GatewayUrl` (optional): Gateway SignalR hub URL. If null, falls back to direct TwitchLib (legacy mode - currently removed)
- `IgnoredUsers`: Array of usernames to ignore in message handlers

### GatewaySettings

- `TwitchChannel`: Twitch channel to monitor
- `TwitchUser`: Bot's Twitch username
- `TwitchAuthKey`: OAuth token for Twitch authentication
- `RedisConnectionString`: Redis connection string for message persistence

## Troubleshooting

### Gateway won't start
- Check Redis is running: `redis-cli ping` should return `PONG`
- Verify `.env` file is in the correct location and has valid Twitch credentials

### Bot can't connect to Gateway
- Ensure Gateway is running and listening on the configured port
- Check `GatewayUrl` in Bot's appsettings.json matches Gateway's address
- Review Gateway logs for connection attempts

### Messages not being processed
- Verify the Gateway is connected to Twitch (check Gateway logs)
- Confirm the Bot is connected to the Gateway (check Bot logs)
- Ensure handler services are registered in `BotModule.cs`

## Development

### Running Tests

```bash
dotnet test
```

### Building the Solution

```bash
dotnet build
```

## Future Enhancements

- Multiple Bot instances can connect to the same Gateway
- Gateway can handle multiple Twitch channels
- Message history retrieval from Redis
- Health monitoring and metrics
- Load balancing across multiple Gateway instances
