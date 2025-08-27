using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot;

public class ChatBot : IComfyBot
{
    private readonly ILogger<ChatBot> logger;
    private readonly IServiceProvider serviceProvider;

    private readonly ITwitchClient twitchClient;

    public ChatBot(ITwitchClient twitchClient,
                   ILogger<ChatBot> logger,
                   IServiceProvider serviceProvider)
    {
        this.twitchClient = twitchClient;
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    public void Run()
    {
        this.InitializeClient();
    }

    private void InitializeClient()
    {
        try
        {
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

    [ExcludeFromCodeCoverage]
    private void OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        using IServiceScope serviceScope = this.serviceProvider.CreateScope();
        IEnumerable<ICommandHandler> commandHandlers = serviceScope.ServiceProvider.GetServices<ICommandHandler>();

        foreach (ICommandHandler handler in commandHandlers)
        {
            try
            {
                handler.Handle(e.Command.Wrap());
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
        using IServiceScope serviceScope = this.serviceProvider.CreateScope();
        IEnumerable<IChatMessageHandler> messageHandlers = serviceScope.ServiceProvider.GetServices<IChatMessageHandler>();

        foreach (IChatMessageHandler handler in messageHandlers)
        {
            try
            {
                handler.Handle(e.ChatMessage.Wrap());
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