namespace ComfyBot.Application.Tests.Bot.Commands
{
    using ComfyBot.Application.Bot.Commands;
    using ComfyBot.Application.Bot.Wrappers;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class TestCommandHandlerTests
    {
        private Mock<ITwitchClient> client;

        private TestCommandHandler handler;
        private Mock<IConfiguration> configuration;
        private Mock<IChatCommand> chatCommand;

        [SetUp]
        public void Setup()
        {
            this.configuration = new Mock<IConfiguration>();
            this.client = new Mock<ITwitchClient>();
            this.chatCommand = new Mock<IChatCommand>();

            this.handler = new TestCommandHandler(this.configuration.Object);
        }

        [TestCase("Test", 1)]
        [TestCase("test", 1)]
        [TestCase("other", 0)]
        public void HandleShouldSendOkayMessage(string command, int calls)
        {
            this.chatCommand.Setup(c => c.CommandText).Returns(command);

            this.handler.Handle(this.client.Object, this.chatCommand.Object);

            this.client.Verify(c => c.SendMessage((string) null, "All working. You're good to go!", false), Times.Exactly(calls));
        }
    }
}