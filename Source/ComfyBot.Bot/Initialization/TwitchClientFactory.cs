namespace ComfyBot.Bot.Initialization
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using TwitchLib.Client;
    using TwitchLib.Client.Interfaces;
    using TwitchLib.Client.Models;
    using TwitchLib.Communication.Clients;
    using TwitchLib.Communication.Models;

    [ExcludeFromCodeCoverage]
    public class TwitchClientFactory : ITwitchClientFactory
    {
        public ITwitchClient Create(string userName, string password, string channel)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(userName, password);
            ClientOptions clientOptions = new ClientOptions { MessagesAllowedInPeriod = 100, ThrottlingPeriod = TimeSpan.FromSeconds(30) };
            WebSocketClient websocketClient = new WebSocketClient(clientOptions);
            TwitchClient twitchClient = new TwitchClient(websocketClient);
            twitchClient.Initialize(credentials, channel);

            return twitchClient;
        }
    }
}