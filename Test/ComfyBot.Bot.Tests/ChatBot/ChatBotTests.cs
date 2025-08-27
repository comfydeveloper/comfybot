using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot;

[TestFixture]
public class ChatBotTests
{
    private ITwitchClient client;

    private BotSettings settings;
    private Bot.ChatBot.ChatBot chatBot;
    private IServiceProvider serviceProvider;

    [SetUp]
    public void Setup()
    {
        this.client = Substitute.For<ITwitchClient>();

        this.serviceProvider = Substitute.For<IServiceProvider>();

        var logger = Substitute.For<ILogger<Bot.ChatBot.ChatBot>>();

        this.settings = new BotSettings();
        IOptions<BotSettings> options = Substitute.For<IOptions<BotSettings>>();
        options.Value.Returns(this.settings);

        this.chatBot = new Bot.ChatBot.ChatBot(this.client, logger, this.serviceProvider);
    }

    [TestCase("user1", "password1", "channel1")]
    [TestCase("user2", "password2", "channel2")]
    public void RunShouldInitializeClient(string username, string password, string channel)
    {
        this.settings.User = username;
        this.settings.AuthKey = password;
        this.settings.Channel = channel;

        this.chatBot.Run();

        this.client.Received().Connect();
    }
}