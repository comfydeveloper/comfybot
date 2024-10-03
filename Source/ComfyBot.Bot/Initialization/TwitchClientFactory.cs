using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace ComfyBot.Bot.Initialization;

[ExcludeFromCodeCoverage]
public class TwitchClientFactory : ITwitchClientFactory
{
    private readonly ILogger<TwitchClientFactory> logger;
    private static TwitchClient twitchClient;
    private readonly BotSettings settings;

    public TwitchClientFactory(IOptions<BotSettings> settings, ILogger<TwitchClientFactory> logger)
    {
        this.logger = logger;
        this.settings = settings.Value;
    }

    public ITwitchClient Create()
    {
        this.AssertSettingsAreProvided();

        if (twitchClient == null)
        {
            string userName = this.settings.User;
            string password = this.settings.AuthKey;
            string channel = this.settings.Channel;

            ConnectionCredentials credentials = new(userName, password);
            ClientOptions clientOptions = new() { MessagesAllowedInPeriod = 100, ThrottlingPeriod = TimeSpan.FromSeconds(30) };
            WebSocketClient websocketClient = new(clientOptions);
            twitchClient = new TwitchClient(websocketClient);
            twitchClient.Initialize(credentials, channel);
        }

        return twitchClient;
    }

    private void AssertSettingsAreProvided()
    {
        if (string.IsNullOrWhiteSpace(this.settings.Channel))
        {
            this.logger.LogError("Channel is not set in the configuration.");
            throw new InvalidOperationException("Channel is not set in the configuration.");
        }

        if (string.IsNullOrWhiteSpace(this.settings.AuthKey))
        {
            this.logger.LogError("AuthKey is not set in the configuration.");
            throw new InvalidOperationException("AuthKey is not set in the configuration.");
        }

        if (string.IsNullOrWhiteSpace(this.settings.User))
        {
            this.logger.LogError("Bot user is not set in the configuration.");
            throw new InvalidOperationException("Bot user is not set in the configuration.");
        }
    }
}