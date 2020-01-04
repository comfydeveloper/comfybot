namespace ComfyBot.Application.Tests.Bot
{
    using ComfyBot.Application.Bot;
    using ComfyBot.Application.Bot.Initialization;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class ComfyBotTests
    {
        private Mock<IConfiguration> configuration;
        private Mock<ITwitchClientFactory> clientFactory;
        private Mock<ITwitchClient> client;

        private ComfyBot comfyBot;

        [SetUp]
        public void Setup()
        {
            this.configuration = new Mock<IConfiguration>();
            this.clientFactory = new Mock<ITwitchClientFactory>();
            this.client = new Mock<ITwitchClient>();
            this.clientFactory.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(this.client.Object);

            this.comfyBot = new ComfyBot(this.configuration.Object, this.clientFactory.Object);
        }

        [TestCase("user1", "password1", "channel1")]
        [TestCase("user2", "password2", "channel2")]
        public void RunShouldInitializeClient(string username, string password, string channel)
        {
            this.configuration.Setup(c => c["user"]).Returns(username);
            this.configuration.Setup(c => c["password"]).Returns(password);
            this.configuration.Setup(c => c["channel"]).Returns(channel);

            this.comfyBot.Run();

            this.clientFactory.Verify(f => f.Create(username, password));
            this.client.Verify(c => c.JoinChannel(channel, false));
            this.client.Verify(c => c.Connect());
        }
    }
}