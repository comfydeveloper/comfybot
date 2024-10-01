using ComfyBot.Bot.Scaffolding;
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
    private static TwitchClient twitchClient;
    private readonly BotSettings settings;

    public TwitchClientFactory(IOptions<BotSettings> settings)
    {
        this.settings = settings.Value;
    }

    public ITwitchClient Create()
    {
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
}