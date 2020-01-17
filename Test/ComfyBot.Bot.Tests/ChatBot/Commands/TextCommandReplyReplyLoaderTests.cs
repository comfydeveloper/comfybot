namespace ComfyBot.Bot.Tests.ChatBot.Commands
{
    using System;

    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class TextCommandReplyReplyLoaderTests
    {
        private Mock<IRepository<TextCommand>> repository;
        private Mock<IChatCommand> chatCommand;

        private TextCommand textCommand;

        private TextCommandReplyReplyLoader replyLoader;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<IRepository<TextCommand>>();
            this.chatCommand = new Mock<IChatCommand>();
            this.chatCommand.Setup(c => c.ChatMessage).Returns(new Mock<IChatMessage>().Object);

            this.textCommand = new TextCommand();

            this.replyLoader = new TextCommandReplyReplyLoader(this.repository.Object);
        }

        [TestCase("command1", "command1", "reply1")]
        [TestCase("command2", "CoMMaND2", "reply2")]
        public void TryGetReplyShouldReturnReplyForMatchingCommand(string command, string textCommandText, string replyText)
        {
            this.chatCommand.Setup(c => c.CommandText).Returns(command);
            this.textCommand.Command = textCommandText;
            this.textCommand.Replies.Add(replyText);

            bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand.Object, out string resultText);

            Assert.IsTrue(result);
            Assert.AreEqual(replyText, resultText);
        }

        [TestCase("message1 {{user}}", "userName1", "message1 userName1")]
        [TestCase("message2 {{user}}", "userName2", "message2 userName2")]
        public void TryGetReplyShouldReplaceUser(string replyText, string userName, string expectedReply)
        {
            this.chatCommand.Setup(c => c.CommandText).Returns("command");
            this.chatCommand.Setup(c => c.ChatMessage.UserName).Returns(userName);
            this.textCommand.Command = "command";
            this.textCommand.Replies.Add(replyText);

            bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand.Object, out string resultText);

            Assert.IsTrue(result);
            Assert.AreEqual(expectedReply, resultText);
        }

        [TestCase("command1", "command2")]
        [TestCase("command2", "command1")]
        public void TryGetReplyShouldReturnFalseWhenMismatchingCommand(string command, string textCommandText)
        {
            this.chatCommand.Setup(c => c.CommandText).Returns(command);
            this.textCommand.Command = textCommandText;

            bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand.Object, out string resultText);

            Assert.IsFalse(result);
            Assert.IsNull(resultText);
        }

        [Test]
        public void TryGetResponseShouldSetLastUsageDateIfMatchWasFound()
        {
            this.chatCommand.Setup(m => m.CommandText).Returns("command");
            this.textCommand.Command = "command";
            this.textCommand.Replies.Add("response");

            this.replyLoader.TryGetReply(this.textCommand, this.chatCommand.Object, out string response);

            Assert.That(this.textCommand.LastUsed, Is.EqualTo(DateTime.Now).Within(2).Seconds);
            this.repository.Verify(r => r.AddOrUpdate(this.textCommand));
        }

        [TestCase(10)]
        [TestCase(20)]
        public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
        {
            this.textCommand.LastUsed = DateTime.Now.AddSeconds(-timeout + 1);
            this.textCommand.TimeoutInSeconds = timeout;

            bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand.Object, out string response);

            Assert.IsNull(response);
            Assert.IsFalse(result);
        }
    }
}