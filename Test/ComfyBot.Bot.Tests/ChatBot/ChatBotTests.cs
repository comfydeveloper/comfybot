namespace ComfyBot.Bot.Tests.ChatBot
{
    using ComfyBot.Bot.ChatBot;
    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Bot.Initialization;
    using ComfyBot.Settings;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Events;
    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class ChatBotTests
    {
        private Mock<ITwitchClientFactory> clientFactory;
        private Mock<ITwitchClient> client;

        private Mock<ICommandHandler> commandHandler1;
        private Mock<ICommandHandler> commandHandler2;

        private ChatBot chatBot;

        [SetUp]
        public void Setup()
        {
            this.clientFactory = new Mock<ITwitchClientFactory>();
            this.client = new Mock<ITwitchClient>();
            this.clientFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(this.client.Object);

            this.commandHandler1 = new Mock<ICommandHandler>();
            this.commandHandler2 = new Mock<ICommandHandler>();
            ICommandHandler[] commandHandlers = { this.commandHandler1.Object, this.commandHandler2.Object };

            this.chatBot = new ChatBot(this.clientFactory.Object, commandHandlers);
        }

        [TestCase("user1", "password1", "channel1")]
        [TestCase("user2", "password2", "channel2")]
        public void RunShouldInitializeClient(string username, string password, string channel)
        {
            ApplicationSettings.Default.User = username;
            ApplicationSettings.Default.Password = password;
            ApplicationSettings.Default.Channel = channel;

            this.chatBot.Run();

            this.clientFactory.Verify(f => f.Create(username, password, channel));
            this.client.Verify(c => c.Connect());
        }

        [Test]
        public void RunShouldRegisterCommandHandlers()
        {
            this.chatBot.Run();
            OnChatCommandReceivedArgs args = new OnChatCommandReceivedArgs();

            this.client.Raise(mock => mock.OnChatCommandReceived += null, args);

            this.commandHandler1.Verify(h => h.Handle(this.client.Object, It.IsAny<IChatCommand>()));
            this.commandHandler2.Verify(h => h.Handle(this.client.Object, It.IsAny<IChatCommand>()));
        }
    }
}