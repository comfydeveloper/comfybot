namespace ComfyBot.Bot.ChatBot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.Extensions;
    using ComfyBot.Bot.Initialization;
    using ComfyBot.Settings;

    using TwitchLib.Client.Events;
    using TwitchLib.Client.Interfaces;

    public class ChatBot : IComfyBot
    {
        private readonly ITwitchClientFactory twitchClientFactory;
        private readonly IEnumerable<ICommandHandler> commandHandlers;

        private ITwitchClient twitchClient;

        public ChatBot(ITwitchClientFactory twitchClientFactory,
                        IEnumerable<ICommandHandler> commandHandlers)
        {
            this.twitchClientFactory = twitchClientFactory;
            this.commandHandlers = commandHandlers;
        }

        public void Run()
        {
            this.InitializeClient();

            Log("Bot initialized.");
        }

        private void InitializeClient()
        {
            this.Logon();
            this.RegisterHandlers();
            this.Connect();
        }

        private void RegisterHandlers()
        {
            this.twitchClient.OnChatCommandReceived += this.OnCommandReceived;
            this.twitchClient.OnLog += OnLog;
            this.twitchClient.OnConnected += OnConnected;
            this.twitchClient.OnJoinedChannel += OnJoinedChannel;
        }

        private void Connect()
        {
            this.twitchClient.Connect();
        }

        private void Logon()
        {
            this.twitchClient = this.twitchClientFactory.Create(ApplicationSettings.Default.User,
                                                                ApplicationSettings.Default.Password,
                                                                ApplicationSettings.Default.Channel);
        }

        private void OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            foreach (ICommandHandler handler in commandHandlers)
            {
                handler.Handle(this.twitchClient, e.Command.Wrap());
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