# ComfyBot Gateway

The Gateway service connects to Twitch chat, receives messages/commands, distributes them to connected SignalR clients, and stores them in Redis for history retrieval.

## Configuration

### Environment Variables

The Gateway uses environment variables for configuration to keep sensitive credentials out of source control.

**For local development:**

1. Copy the `.env.example` file to `bin\Debug\net10.0\.env`
2. Fill in your actual values:

```bash
# Twitch Configuration
Gateway__TwitchChannel=yourchannelname
Gateway__TwitchUser=yourbotusername
Gateway__TwitchAuthKey=oauth:your_token_here

# Redis Configuration
Gateway__RedisConnectionString=localhost:6379
```

**Get your Twitch OAuth token:** https://twitchapps.com/tmi/

### Configuration Priority

The application loads configuration in this order (later sources override earlier ones):
1. `appsettings.json` (base configuration)
2. `appsettings.Development.json` (development overrides)
3. `.env` file (local environment variables)
4. System environment variables (production/deployment)

## Running the Gateway

### Prerequisites

- .NET 10 SDK
- Redis server (local or remote)
- Twitch account and OAuth token

### Start Redis (if running locally)

```bash
docker run -d -p 6379:6379 redis:latest
```

Or use Windows Subsystem for Linux (WSL) with Redis installed.

### Run the Gateway

```bash
dotnet run --project Source\ComfyBot.Gateway\ComfyBot.Gateway.csproj
```

The Gateway will:
- Load configuration from `.env` file in the bin directory
- Connect to Twitch chat
- Start SignalR hub on `/chat`
- Expose health endpoint on `/health`

## SignalR Hub Endpoints

### `/chat` - Main SignalR Hub

**Client Methods (receive from server):**
- `ReceiveMessage(MessageReceivedEvent)` - Incoming chat message
- `ReceiveCommand(CommandReceivedEvent)` - Incoming command

**Server Methods (invoke from client):**
- `SendMessage(SendMessageRequest)` → `SendMessageResponse` - Send message to Twitch chat

## Security Notes

- **Never commit `.env` files** - They contain sensitive credentials
- The `.env` file is already in `.gitignore`
- For production, use environment variables or secure secrets management (Azure Key Vault, AWS Secrets Manager, etc.)
