namespace ComfyBot.Application.Tests.Bot
{
    using System.Collections.Generic;

    using ComfyBot.Application.Bot;
    using ComfyBot.Application.Bot.Commands;
    using ComfyBot.Application.Bot.Extensions;
    using ComfyBot.Application.Bot.Initialization;
    using ComfyBot.Application.Bot.Wrappers;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Events;
    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class ComfyBotTests
    {
        private Mock<IConfigurationSection> credentialSection;
        private Mock<IConfiguration> configuration;
        private Mock<ITwitchClientFactory> clientFactory;
        private Mock<ITwitchClient> client;

        private Mock<ICommandHandler> commandHandler1;
        private Mock<ICommandHandler> commandHandler2;

        private ComfyBot comfyBot;

        [SetUp]
        public void Setup()
        {
            this.credentialSection = new Mock<IConfigurationSection>();
            this.configuration = new Mock<IConfiguration>();
            this.clientFactory = new Mock<ITwitchClientFactory>();
            this.client = new Mock<ITwitchClient>();
            this.clientFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(this.client.Object);
            this.configuration.Setup(c => c.GetSection("credentials")).Returns(this.credentialSection.Object);

            this.commandHandler1 = new Mock<ICommandHandler>();
            this.commandHandler2 = new Mock<ICommandHandler>();
            ICommandHandler[] commandHandlers = { this.commandHandler1.Object, this.commandHandler2.Object };

            this.comfyBot = new ComfyBot(this.configuration.Object, this.clientFactory.Object, commandHandlers);
        }

        [TestCase("user1", "password1", "channel1")]
        [TestCase("user2", "password2", "channel2")]
        public void RunShouldInitializeClient(string username, string password, string channel)
        {
            this.credentialSection.Setup(c => c["user"]).Returns(username);
            this.credentialSection.Setup(c => c["password"]).Returns(password);
            this.configuration.Setup(c => c["channel"]).Returns(channel);

            this.comfyBot.Run();

            this.clientFactory.Verify(f => f.Create(username, password, channel));
            this.client.Verify(c => c.Connect());
        }

        [Test]
        public void RunShouldRegisterCommandHandlers()
        {
            this.comfyBot.Run();
            OnChatCommandReceivedArgs args = new OnChatCommandReceivedArgs();

            this.client.Raise(mock => mock.OnChatCommandReceived += null, args);

            this.commandHandler1.Verify(h => h.Handle(this.client.Object, It.IsAny<IChatCommand>()));
        }
    }
}