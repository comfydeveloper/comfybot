using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.Extensions;
using ComfyBot.Bot.Initialization;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot;

public class ChatBot : IComfyBot
{
    private readonly ITwitchClientFactory twitchClientFactory;
    private readonly IEnumerable<ICommandHandler> commandHandlers;
    private readonly IEnumerable<IMessageHandler> messageHandlers;
    private readonly ILogger<ChatBot> logger;
    private readonly BotSettings settings;

    private ITwitchClient twitchClient;

    public ChatBot(ITwitchClientFactory twitchClientFactory,
        IEnumerable<ICommandHandler> commandHandlers,
        IEnumerable<IMessageHandler> messageHandlers,
        IOptions<BotSettings> botSettings,
        ILogger<ChatBot> logger)
    {
        this.twitchClientFactory = twitchClientFactory;
        this.commandHandlers = commandHandlers;
        this.messageHandlers = messageHandlers;
        this.logger = logger;
        this.settings = botSettings.Value;
    }

    public void Run()
    {
        if (IsBotReady())
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
        return !string.IsNullOrEmpty(this.settings.Channel)
               && !string.IsNullOrEmpty(this.settings.AuthKey)
               && !string.IsNullOrEmpty(this.settings.User);
    }

    private void InitializeClient()
    {
        try
        {
            this.Logon();
            this.RegisterHandlers();
            this.Connect();
            Log("Bot initialized.");
        }
        catch (Exception ex)
        {
            this.logger.LogCritical(ex, "Failed to initialize bot");
            throw;
        }
    }

    private void Connect()
    {
        this.twitchClient.Connect();
    }

    private void RegisterHandlers()
    {
        this.twitchClient.OnChatCommandReceived += this.OnCommandReceived;
        this.twitchClient.OnMessageReceived += this.OnMessageReceived;
        this.twitchClient.OnLog += OnLog;
        this.twitchClient.OnConnected += this.OnConnected;
        this.twitchClient.OnJoinedChannel += this.OnJoinedChannel;
    }

    private void Logon()
    {
        this.twitchClient = this.twitchClientFactory.Create();
    }

    [ExcludeFromCodeCoverage]
    private void OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        foreach (ICommandHandler handler in this.commandHandlers)
        {
            try
            {
                handler.Handle(this.twitchClient, e.Command.Wrap());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to handle command {@CommandText}", e.Command.CommandText);
                Log($"Failed to handle command {e.Command.CommandText} - {ex.Message}");
            }

        }
    }

    [ExcludeFromCodeCoverage]
    private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        foreach (IMessageHandler handler in this.messageHandlers)
        {
            try
            {
                handler.Handle(this.twitchClient, e.ChatMessage.Wrap());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to handle message {@Message}", e.ChatMessage.Message);
            }
        }
    }

    [ExcludeFromCodeCoverage]
    private void OnConnected(object sender, OnConnectedArgs e)
    {
        try
        {
            Log("Successfully connected!");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to log successful connection");
        }
    }

    [ExcludeFromCodeCoverage]
    private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        try
        {
            Log($"Successfully joined channel {e.Channel}.");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to log successful channel join");
        }
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