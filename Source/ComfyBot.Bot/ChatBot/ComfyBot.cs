namespace ComfyBot.Bot.ChatBot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using global::ComfyBot.Bot.ChatBot.Commands;
    using global::ComfyBot.Bot.Extensions;
    using global::ComfyBot.Bot.Initialization;
    using global::ComfyBot.Settings;

    using TwitchLib.Client.Events;
    using TwitchLib.Client.Interfaces;

    public class ComfyBot : IComfyBot
    {
        private readonly ITwitchClientFactory twitchClientFactory;
        private readonly IEnumerable<ICommandHandler> commandHandlers;

        private ITwitchClient twitchClient;

        public ComfyBot(ITwitchClientFactory twitchClientFactory,
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
            this.twitchClient.OnLog += this.OnLog;
            this.twitchClient.OnConnected += this.OnConnected;
            this.twitchClient.OnJoinedChannel += this.OnJoinedChannel;
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
            Log($"Received command {e.Command.CommandText} from user {e.Command.ChatMessage.Username}");

            foreach (ICommandHandler handler in commandHandlers)
            {
                handler.Handle(this.twitchClient, e.Command.Wrap());
            }
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            Log($"Successfully connected!");
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Log($"Successfully joined channel {e.Channel}.");
        }

        [ExcludeFromCodeCoverage]
        private void OnLog(object sender, OnLogArgs e)
        {
            Log($"{e.BotUsername} - {e.Data}");
        }

        private static void Log(string message)
        {
            Console.Write($"{DateTime.Now}: {message}");
        }
    }
}