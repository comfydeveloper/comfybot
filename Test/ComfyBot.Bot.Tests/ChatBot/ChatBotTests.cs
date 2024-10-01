using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.Initialization;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot;

[TestFixture]
public class ChatBotTests
{
    private Mock<ITwitchClientFactory> clientFactory;
    private Mock<ITwitchClient> client;

    private Mock<ICommandHandler> commandHandler1;
    private Mock<ICommandHandler> commandHandler2;
    private Mock<IMessageHandler> messageHandler1;
    private Mock<IMessageHandler> messageHandler2;

    private BotSettings settings;
    private Bot.ChatBot.ChatBot chatBot;

    [SetUp]
    public void Setup()
    {
        this.clientFactory = Substitute.For<ITwitchClientFactory>();
        this.client = Substitute.For<ITwitchClient>();
        this.clientFactory.Setup(f => f.Create()).Returns(this.client);

        this.commandHandler1 = Substitute.For<ICommandHandler>();
        this.commandHandler2 = Substitute.For<ICommandHandler>();
        ICommandHandler[] commandHandlers = [this.commandHandler1, this.commandHandler2];

        this.messageHandler1 = Substitute.For<IMessageHandler>();
        this.messageHandler2 = Substitute.For<IMessageHandler>();
        IMessageHandler[] messageHandlers = [this.messageHandler1, this.messageHandler2];
        var logger = Substitute.For<ILogger<Bot.ChatBot.ChatBot>>();

        this.settings = new BotSettings();
        Mock<IOptions<BotSettings>> options = new();
        options.Setup(x => x.Value).Returns(this.settings);

        this.chatBot = new Bot.ChatBot.ChatBot(this.clientFactory, commandHandlers, messageHandlers, options, logger);
    }

    [TestCase("user1", "password1", "channel1")]
    [TestCase("user2", "password2", "channel2")]
    public void RunShouldInitializeClient(string username, string password, string channel)
    {
        this.settings.User = username;
        this.settings.AuthKey = password;
        this.settings.Channel = channel;

        this.chatBot.Run();

        this.clientFactory.Verify(f => f.Create());
        this.client.Verify(c => c.Connect());
    }

    [Test]
    public void RunShouldExitWhenSettingsAreNotReady()
    {
        this.settings.User = string.Empty;
        this.settings.AuthKey = string.Empty;
        this.settings.Channel = string.Empty;

        this.chatBot.Run();

        this.clientFactory.Verify(f => f.Create(), Times.Never);
    }
}