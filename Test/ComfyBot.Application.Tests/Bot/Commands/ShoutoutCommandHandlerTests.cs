namespace ComfyBot.Application.Tests.Bot.Commands
{
    using ComfyBot.Application.Bot.Commands;
    using ComfyBot.Application.Bot.Wrappers;
    using ComfyBot.Application.Data;
    using ComfyBot.Application.Data.Models;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class ShoutoutCommandHandlerTests
    {
        private Mock<IConfiguration> configuration;
        private Mock<IRepository<Shoutout>> repository;
        private Mock<ITwitchClient> client;
        private Mock<IChatCommand> command;
        private Mock<IChatMessage> message;

        private ShoutoutCommandHandler handler;

        [SetUp]
        public void Setup()
        {
            this.configuration = new Mock<IConfiguration>();
            this.repository = new Mock<IRepository<Shoutout>>();
            this.client = new Mock<ITwitchClient>();
            this.command = new Mock<IChatCommand>();
            this.message = new Mock<IChatMessage>();
            this.command.Setup(c => c.ChatMessage).Returns(message.Object);

            this.handler = new ShoutoutCommandHandler(this.configuration.Object, this.repository.Object);
        }

        [TestCase("so", "set test1 some text", "test1", "some text")]
        [TestCase("So", "Set test2 some other text", "test2", "some other text")]
        public void HandleShouldDelegateToRepository(string commandText, string commandParameters, string id, string shoutoutText)
        {
            this.command.Setup(c => c.ArgumentsAsString).Returns(commandParameters);
            this.command.Setup(c => c.CommandText).Returns(commandText);
            this.message.Setup(m => m.IsBroadcaster).Returns(true);
            Shoutout callback = null;
            this.repository.Setup(r => r.AddOrUpdate(It.IsAny<Shoutout>())).Callback<Shoutout>(r => callback = r);

            this.handler.Handle(this.client.Object, this.command.Object);

            Assert.AreEqual(id, callback.Id);
            Assert.AreEqual(shoutoutText, callback.ShoutoutText);
        }

        [TestCase("test1", "some text")]
        [TestCase("test2", "some other text")]
        public void HandleShouldDelegateToRepository(string id, string shoutoutText)
        {
            this.command.Setup(c => c.ArgumentsAsString).Returns(id);
            this.command.Setup(c => c.CommandText).Returns("so");
            Shoutout model = new Shoutout { ShoutoutText = shoutoutText };
            this.repository.Setup(r => r.Get(id)).Returns(model);
            string callback = null;
            this.client.Setup(c => c.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false)).Callback<string, string, bool>((a, b, c) => callback = b);

            this.handler.Handle(this.client.Object, this.command.Object);

            Assert.AreEqual($@"▬▬☆✼★▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬★✼☆▬▬
                                                {shoutoutText}      
                                                ▬▬☆✼★━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━★✼☆▬▬", callback);
        }

        [TestCase(true, true, 1)]
        [TestCase(false, true, 1)]
        [TestCase(true, false, 1)]
        [TestCase(false, false, 0)]
        public void HandleShouldOnlySetForModeratorsOrBroadcaster(bool isBroadcaster, bool isModerator, int expectedCallCount)
        {
            this.command.Setup(c => c.ArgumentsAsString).Returns("set test test");
            this.command.Setup(c => c.CommandText).Returns("so");
            this.message.Setup(m => m.IsBroadcaster).Returns(isBroadcaster);
            this.message.Setup(m => m.IsModerator).Returns(isModerator);

            this.handler.Handle(this.client.Object, this.command.Object);

            this.repository.Verify(r => r.AddOrUpdate(It.IsAny<Shoutout>()), Times.Exactly(expectedCallCount));
        }
    }
}