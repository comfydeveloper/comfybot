# ComfyBot Docker Compose Setup

This guide explains how to run both the ComfyBot Gateway and Application services using Docker Compose.

## Prerequisites

- Docker and Docker Compose installed (version 3.8 or higher)
- A Twitch account and API credentials (channel, username, and auth key)

## Configuration

### 1. Create `.env` file

Copy the `.env.example` file to `.env` and fill in your configuration:

```bash
cp .env.example .env
```

Edit `.env` with your values:

```env
# PostgreSQL Configuration
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_secure_password_here
POSTGRES_DB=ComfybotDb

# Twitch Gateway Configuration
GATEWAY_TWITCH_CHANNEL=your_twitch_channel_here
GATEWAY_TWITCH_USER=your_twitch_user_here
GATEWAY_TWITCH_AUTH_KEY=your_twitch_auth_key_here

# Bot Configuration (Optional - defaults shown)
BOT_IGNORED_USERS=Streamelements
```

### Environment Variables

| Variable | Description | Required | Example |
|----------|-------------|----------|---------|
| `POSTGRES_USER` | PostgreSQL username | Yes | `postgres` |
| `POSTGRES_PASSWORD` | PostgreSQL password | Yes | `SecurePassword123!` |
| `POSTGRES_DB` | PostgreSQL database name | Yes | `ComfybotDb` |
| `GATEWAY_TWITCH_CHANNEL` | Your Twitch channel name | Yes | `your_channel` |
| `GATEWAY_TWITCH_USER` | Your Twitch bot username | Yes | `your_bot_account` |
| `GATEWAY_TWITCH_AUTH_KEY` | Your Twitch OAuth token | Yes | `oauth:xxxxxxxxxxxx` |
| `BOT_IGNORED_USERS` | Comma-separated usernames to ignore | No | `Streamelements,Nightbot` |

## Running the Services

### Start all services

```bash
docker-compose up -d
```

This command will:
1. Build Docker images for Gateway and Application
2. Start PostgreSQL database
3. Start Redis cache
4. Start ComfyBot Gateway (http://localhost:5125)
5. Start ComfyBot Application (http://localhost:5000)

### View logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f gateway
docker-compose logs -f application
```

### Stop services

```bash
docker-compose down
```

### Stop and remove volumes

```bash
docker-compose down -v
```

## Service Details

### ComfyBot Gateway (http://localhost:5010)
- ASP.NET Core WebAPI with SignalR
- Connects to Twitch chat
- Caches messages in Redis
- Broadcasts messages to connected clients via SignalR
- Health check endpoint: `http://localhost:5010/health`

### ComfyBot Application (http://localhost:5000)
- Blazor Server web application
- Manages commands, responses, and variables
- Persists data to PostgreSQL
- Communicates with Gateway service

### PostgreSQL (localhost:6000)
- Database server
- Default credentials: `postgres:your_password_here`
- Database name: `ComfybotDb`

### Redis (localhost:5500)
- In-memory cache
- Used by Gateway for message history

## Troubleshooting

### Services fail to start
Check logs: `docker-compose logs`

### Database connection errors
Ensure PostgreSQL container is healthy: `docker-compose ps`

### Gateway not connecting to Twitch
Verify your Twitch credentials in `.env` file

### Application shows database errors
Check that PostgreSQL migrations ran successfully in logs

## Development

To rebuild images after code changes:

```bash
docker-compose up -d --build
```

## Next Steps

1. Access the ComfyBot Application: http://localhost:5000
2. Create text commands, responses, and variables
3. The Gateway will automatically forward Twitch chat messages
