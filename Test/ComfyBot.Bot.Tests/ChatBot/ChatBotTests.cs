using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot;

[TestFixture]
public class ChatBotTests
{
    private ITwitchClient client;

    private ICommandHandler commandHandler1;
    private ICommandHandler commandHandler2;
    private IChatMessageHandler chatMessageHandler1;
    private IChatMessageHandler chatMessageHandler2;

    private BotSettings settings;
    private Bot.ChatBot.ChatBot chatBot;

    [SetUp]
    public void Setup()
    {
        this.client = Substitute.For<ITwitchClient>();

        this.commandHandler1 = Substitute.For<ICommandHandler>();
        this.commandHandler2 = Substitute.For<ICommandHandler>();
        ICommandHandler[] commandHandlers = [this.commandHandler1, this.commandHandler2];

        this.chatMessageHandler1 = Substitute.For<IChatMessageHandler>();
        this.chatMessageHandler2 = Substitute.For<IChatMessageHandler>();
        IChatMessageHandler[] messageHandlers = [this.chatMessageHandler1, this.chatMessageHandler2];
        var logger = Substitute.For<ILogger<Bot.ChatBot.ChatBot>>();

        this.settings = new BotSettings();
        IOptions<BotSettings> options = Substitute.For<IOptions<BotSettings>>();
        options.Value.Returns(this.settings);

        this.chatBot = new Bot.ChatBot.ChatBot(this.client, commandHandlers, messageHandlers, logger);
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