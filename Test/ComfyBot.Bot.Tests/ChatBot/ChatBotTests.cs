﻿using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.Initialization;
using ComfyBot.Settings;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot;

[TestFixture]
public class ChatBotTests
{
    private Mock<ITwitchClientFactory> clientFactory;
    private Mock<ITwitchClient> client;
    private Mock<IChattersCache> chattersCache;

    private Mock<ICommandHandler> commandHandler1;
    private Mock<ICommandHandler> commandHandler2;
    private Mock<IMessageHandler> messageHandler1;
    private Mock<IMessageHandler> messageHandler2;

    private Bot.ChatBot.ChatBot chatBot;

    [SetUp]
    public void Setup()
    {
        clientFactory = new Mock<ITwitchClientFactory>();
        client = new Mock<ITwitchClient>();
        clientFactory.Setup(f => f.Create()).Returns(client.Object);
        chattersCache = new Mock<IChattersCache>();

        commandHandler1 = new Mock<ICommandHandler>();
        commandHandler2 = new Mock<ICommandHandler>();
        ICommandHandler[] commandHandlers = { commandHandler1.Object, commandHandler2.Object };

        messageHandler1 = new Mock<IMessageHandler>();
        messageHandler2 = new Mock<IMessageHandler>();
        IMessageHandler[] messageHandlers = { messageHandler1.Object, messageHandler2.Object };
        var logger = new Mock<ILogger<Bot.ChatBot.ChatBot>>();

        chatBot = new Bot.ChatBot.ChatBot(clientFactory.Object, commandHandlers, messageHandlers, chattersCache.Object, logger.Object);
    }

    [TestCase("user1", "password1", "channel1")]
    [TestCase("user2", "password2", "channel2")]
    public void RunShouldInitializeClient(string username, string password, string channel)
    {
        ApplicationSettings.Default.User = username;
        ApplicationSettings.Default.AuthKey = password;
        ApplicationSettings.Default.Channel = channel;

        chatBot.Run();

        clientFactory.Verify(f => f.Create());
        client.Verify(c => c.Connect());
    }

    [Test]
    public void RunShouldExitWhenSettingsAreNotReady()
    {
        ApplicationSettings.Default.Channel = string.Empty;
        ApplicationSettings.Default.AuthKey = string.Empty;
        ApplicationSettings.Default.User = string.Empty;

        chatBot.Run();

        clientFactory.Verify(f => f.Create(), Times.Never);
    }
}