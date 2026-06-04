using ComfyBot.Gateway.Extensions;
using ComfyBot.Gateway.SignalR;
using ComfyBot.Gateway.Services;
using Microsoft.AspNetCore.SignalR;
using DotNetEnv;

// Load .env file if it exists (for local development)
string envPath = Path.Combine(AppContext.BaseDirectory, ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddGatewayServices(builder.Configuration);

WebApplication app = builder.Build();

app.MapHub<ChatHub>("/chat");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

ITwitchService twitchService = app.Services.GetRequiredService<ITwitchService>();
IRedisService redisService = app.Services.GetRequiredService<IRedisService>();
IHubContext<ChatHub, IChatHubClient> hubContext = app.Services.GetRequiredService<IHubContext<ChatHub, IChatHubClient>>();

twitchService.OnMessageReceived += async (_, messageEvent) =>
{
    app.Logger.LogInformation("Gateway: Received message from Twitch, forwarding to SignalR clients. User: {User}, Text: {Text}", 
        messageEvent.Message.UserName, messageEvent.Message.Text);
    await redisService.StoreMessageAsync(messageEvent);
    await hubContext.Clients.All.ReceiveMessage(messageEvent);
    app.Logger.LogInformation("Gateway: Message forwarded to SignalR clients");
};

twitchService.OnCommandReceived += async (_, commandEvent) =>
{
    app.Logger.LogInformation("Gateway: Received command from Twitch, forwarding to SignalR clients. User: {User}, Command: {Command}", 
        commandEvent.Command.ChatMessage.UserName, commandEvent.Command.CommandText);
    await redisService.StoreCommandAsync(commandEvent);
    await hubContext.Clients.All.ReceiveCommand(commandEvent);
    app.Logger.LogInformation("Gateway: Command forwarded to SignalR clients");
};

IHostApplicationLifetime lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() =>
{
    twitchService.Connect();
});

lifetime.ApplicationStopping.Register(() =>
{
    twitchService.Disconnect();
});

app.Run();
