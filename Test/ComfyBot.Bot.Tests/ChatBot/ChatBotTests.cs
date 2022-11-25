namespace ComfyBot.Bot.Tests.ChatBot
{
    using ComfyBot.Bot.ChatBot;
    using ComfyBot.Bot.ChatBot.Chatters;
    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.ChatBot.Messages;
    using ComfyBot.Bot.Initialization;
    using ComfyBot.Settings;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Interfaces;

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

        private ChatBot chatBot;

        [SetUp]
        public void Setup()
        {
            this.clientFactory = new Mock<ITwitchClientFactory>();
            this.client = new Mock<ITwitchClient>();
            this.clientFactory.Setup(f => f.Create()).Returns(this.client.Object);
            this.chattersCache = new Mock<IChattersCache>();

            this.commandHandler1 = new Mock<ICommandHandler>();
            this.commandHandler2 = new Mock<ICommandHandler>();
            ICommandHandler[] commandHandlers = { this.commandHandler1.Object, this.commandHandler2.Object };

            this.messageHandler1 = new Mock<IMessageHandler>();
            this.messageHandler2 = new Mock<IMessageHandler>();
            IMessageHandler[] messageHandlers = { this.messageHandler1.Object, this.messageHandler2.Object };

            this.chatBot = new ChatBot(this.clientFactory.Object, commandHandlers, messageHandlers, this.chattersCache.Object);
        }

        [TestCase("user1", "password1", "channel1")]
        [TestCase("user2", "password2", "channel2")]
        public void RunShouldInitializeClient(string username, string password, string channel)
        {
            ApplicationSettings.Default.User = username;
            ApplicationSettings.Default.AuthKey = password;
            ApplicationSettings.Default.Channel = channel;

            this.chatBot.Run();

            this.clientFactory.Verify(f => f.Create());
            this.client.Verify(c => c.Connect());
        }

        [Test]
        public void RunShouldExitWhenSettingsAreNotReady()
        {
            ApplicationSettings.Default.Channel = string.Empty;
            ApplicationSettings.Default.AuthKey = string.Empty;
            ApplicationSettings.Default.User = string.Empty;

            this.chatBot.Run();

            this.clientFactory.Verify(f => f.Create(), Times.Never);
        }
    }
}