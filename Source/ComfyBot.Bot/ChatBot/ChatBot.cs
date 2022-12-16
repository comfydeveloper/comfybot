namespace ComfyBot.Bot.ChatBot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Chatters;
    using Commands;
    using Messages;
    using Extensions;
    using Initialization;
    using Settings;

    using TwitchLib.Client.Events;
    using TwitchLib.Client.Interfaces;

    public class ChatBot : IComfyBot
    {
        private readonly ITwitchClientFactory twitchClientFactory;
        private readonly IEnumerable<ICommandHandler> commandHandlers;
        private readonly IEnumerable<IMessageHandler> messageHandlers;
        private readonly IChattersCache chattersCache;

        private ITwitchClient twitchClient;

        public ChatBot(ITwitchClientFactory twitchClientFactory,
                       IEnumerable<ICommandHandler> commandHandlers,
                       IEnumerable<IMessageHandler> messageHandlers,
                       IChattersCache chattersCache)
        {
            this.twitchClientFactory = twitchClientFactory;
            this.commandHandlers = commandHandlers;
            this.messageHandlers = messageHandlers;
            this.chattersCache = chattersCache;
        }

        public void Run()
        {
            if (IsBotReady())
            {
                InitializeClient();
            }
            else
            {
                Log("Could not initialize bot. Please make sure to set your configuration in the configuration tab and restart the bot.");
            }
        }

        private bool IsBotReady()
        {
            ApplicationSettings applicationSettings = ApplicationSettings.Default;
            return !string.IsNullOrEmpty(applicationSettings.Channel)
                   && !string.IsNullOrEmpty(applicationSettings.AuthKey)
                   && !string.IsNullOrEmpty(applicationSettings.User);
        }

        private void InitializeClient()
        {
            Logon();
            RegisterHandlers();
            Connect();
            Log("Bot initialized.");
        }

        private void Connect()
        {
            twitchClient.Connect();
        }

        private void RegisterHandlers()
        {
            twitchClient.OnChatCommandReceived += OnCommandReceived;
            twitchClient.OnMessageReceived += OnMessageReceived;
            twitchClient.OnLog += OnLog;
            twitchClient.OnConnected += OnConnected;
            twitchClient.OnJoinedChannel += OnJoinedChannel;
            twitchClient.OnUserJoined += OnUserJoined;
            twitchClient.OnUserLeft += OnUserLeft;
        }

        private void Logon()
        {
            twitchClient = twitchClientFactory.Create();
        }

        [ExcludeFromCodeCoverage]
        private void OnUserLeft(object sender, OnUserLeftArgs e)
        {
            chattersCache.Remove(e.Username);
        }

        [ExcludeFromCodeCoverage]
        private void OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            chattersCache.Add(e.Username);
        }

        [ExcludeFromCodeCoverage]
        private void OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            foreach (ICommandHandler handler in commandHandlers)
            {
                handler.Handle(twitchClient, e.Command.Wrap());
            }
        }

        [ExcludeFromCodeCoverage]
        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            foreach (IMessageHandler handler in messageHandlers)
            {
                handler.Handle(twitchClient, e.ChatMessage.Wrap());
            }
        }

        [ExcludeFromCodeCoverage]
        private static void OnConnected(object sender, OnConnectedArgs e)
        {
            Log($"Successfully connected!");
        }

        [ExcludeFromCodeCoverage]
        private static void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Log($"Successfully joined channel {e.Channel}.");
        }

        [ExcludeFromCodeCoverage]
        private static void OnLog(object sender, OnLogArgs e)
        {
            Log($"{e.BotUsername} - {e.Data}");
        }

        private static void Log(string message)
        {
            Console.Write($"{DateTime.Now}: {message}");
        }
    }
}