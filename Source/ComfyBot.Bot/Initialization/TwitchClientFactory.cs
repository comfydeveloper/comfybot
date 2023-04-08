using System;
using System.Diagnostics.CodeAnalysis;
using ComfyBot.Settings;
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

    public ITwitchClient Create()
    {
        if (twitchClient == null)
        {
            string userName = ApplicationSettings.Default.User;
            string password = ApplicationSettings.Default.AuthKey;
            string channel = ApplicationSettings.Default.Channel;

            ConnectionCredentials credentials = new ConnectionCredentials(userName, password);
            ClientOptions clientOptions = new ClientOptions { MessagesAllowedInPeriod = 100, ThrottlingPeriod = TimeSpan.FromSeconds(30) };
            WebSocketClient websocketClient = new WebSocketClient(clientOptions);
            twitchClient = new TwitchClient(websocketClient);
            twitchClient.Initialize(credentials, channel);
        }

        return twitchClient;
    }
}