using ComfyBot.Bot.Gateway;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ComfyBot.Bot.Tests.ChatBot;

[TestFixture]
public class ChatBotTests
{
    private IGatewayClient gatewayClient;
    private IGatewayEventBridge eventBridge;
    private BotSettings settings;
    private Bot.ChatBot.ChatBot chatBot;

    [SetUp]
    public void Setup()
    {
        this.gatewayClient = Substitute.For<IGatewayClient>();
        this.eventBridge = Substitute.For<IGatewayEventBridge>();

        ILogger<Bot.ChatBot.ChatBot> logger = Substitute.For<ILogger<Bot.ChatBot.ChatBot>>();

        this.settings = new BotSettings();
        IOptions<BotSettings> options = Substitute.For<IOptions<BotSettings>>();
        options.Value.Returns(this.settings);

        this.chatBot = new Bot.ChatBot.ChatBot(this.gatewayClient, this.eventBridge, logger);
    }

    [Test]
    public async Task RunShouldRegisterHandlersAndConnectToGateway()
    {
        await this.chatBot.Run();

        this.eventBridge.Received().RegisterHandlers();
        await this.gatewayClient.Received().ConnectAsync();
    }
}
