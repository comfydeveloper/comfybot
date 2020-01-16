namespace ComfyBot.Bot.Tests.ChatBot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class ShoutoutCommandHandlerTests
    {
        private Mock<IRepository<Shoutout>> repository;
        private Mock<ITwitchClient> client;
        private Mock<IChatCommand> command;
        private Mock<IChatMessage> message;

        private ShoutoutCommandHandler handler;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<IRepository<Shoutout>>();
            this.client = new Mock<ITwitchClient>();
            this.command = new Mock<IChatCommand>();
            this.message = new Mock<IChatMessage>();
            this.command.Setup(c => c.ChatMessage).Returns(message.Object);

            this.handler = new ShoutoutCommandHandler(this.repository.Object);
        }

        [TestCase("test1", "some text")]
        [TestCase("test2", "some other text")]
        public void HandleShouldDelegateToRepository(string id, string shoutoutText)
        {
            this.command.Setup(c => c.ArgumentsAsList).Returns(new List<string> {id});
            this.command.Setup(c => c.CommandText).Returns("so");
            Shoutout model = new Shoutout { Message = shoutoutText };
            this.repository.Setup(r => r.Get(It.IsAny<Expression<Func<Shoutout, bool>>>())).Returns(model);
            string callback = null;
            this.client.Setup(c => c.SendMessage(It.IsAny<string>(), It.IsAny<string>(), false)).Callback<string, string, bool>((a, b, c) => callback = b);

            this.handler.Handle(this.client.Object, this.command.Object);

            Assert.AreEqual($@"▬▬☆✼★▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬★✼☆▬▬
                                                {shoutoutText}      
                                                ▬▬☆✼★━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━★✼☆▬▬", callback);
        }
    }
}