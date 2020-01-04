namespace ComfyBot.Application.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using global::ComfyBot.Application.Bot.Commands;
    using global::ComfyBot.Application.Bot.Extensions;
    using global::ComfyBot.Application.Bot.Initialization;

    using Microsoft.Extensions.Configuration;

    using TwitchLib.Client.Events;
    using TwitchLib.Client.Interfaces;

    public class ComfyBot : IComfyBot
    {
        private readonly IConfiguration configuration;
        private readonly ITwitchClientFactory twitchClientFactory;
        private readonly IEnumerable<ICommandHandler> commandHandlers;

        private ITwitchClient twitchClient;

        public ComfyBot(IConfiguration configuration,
                        ITwitchClientFactory twitchClientFactory,
                        IEnumerable<ICommandHandler> commandHandlers)
        {
            this.configuration = configuration;
            this.twitchClientFactory = twitchClientFactory;
            this.commandHandlers = commandHandlers;
        }

        public void Run()
        {
            this.InitializeClient();
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
        }

        private void Connect()
        {
            this.twitchClient.Connect();
        }

        private void Logon()
        {
            this.twitchClient = this.twitchClientFactory.Create(this.configuration.GetSection("credentials")["user"],
                                                                this.configuration.GetSection("credentials")["password"],
                                                                this.configuration["channel"]);
        }

        private void OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            foreach (ICommandHandler handler in commandHandlers)
            {
                handler.Handle(this.twitchClient, e.Command.Wrap());
            }
        }

        [ExcludeFromCodeCoverage]
        private void OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
        }
    }
}