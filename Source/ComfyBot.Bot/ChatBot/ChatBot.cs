﻿namespace ComfyBot.Bot.ChatBot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.ChatBot.Messages;
    using ComfyBot.Bot.Extensions;
    using ComfyBot.Bot.Initialization;
    using ComfyBot.Settings;

    using TwitchLib.Client.Events;
    using TwitchLib.Client.Interfaces;

    public class ChatBot : IComfyBot
    {
        private readonly ITwitchClientFactory twitchClientFactory;
        private readonly IEnumerable<ICommandHandler> commandHandlers;
        private readonly IEnumerable<IMessageHandler> messageHandlers;

        private ITwitchClient twitchClient;

        public ChatBot(ITwitchClientFactory twitchClientFactory,
                       IEnumerable<ICommandHandler> commandHandlers,
                       IEnumerable<IMessageHandler> messageHandlers)
        {
            this.twitchClientFactory = twitchClientFactory;
            this.commandHandlers = commandHandlers;
            this.messageHandlers = messageHandlers;
        }

        public void Run()
        {
            if (this.IsBotReady())
            {
                this.InitializeClient();
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
            this.Logon();
            this.RegisterHandlers();
            this.Connect();
            Log("Bot initialized.");
        }

        private void RegisterHandlers()
        {
            this.twitchClient.OnChatCommandReceived += this.OnCommandReceived;
            this.twitchClient.OnMessageReceived += this.OnMessageReceived;
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
                                                                ApplicationSettings.Default.AuthKey,
                                                                ApplicationSettings.Default.Channel);
        }

        private void OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            foreach (ICommandHandler handler in commandHandlers)
            {
                handler.Handle(this.twitchClient, e.Command.Wrap());
            }
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            foreach (IMessageHandler handler in this.messageHandlers)
            {
                handler.Handle(this.twitchClient, e.ChatMessage.Wrap());
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