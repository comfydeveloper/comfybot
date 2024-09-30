using System;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Moq;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TextCommandReplyLoaderTests
{
    private Mock<IRepository<TextCommandOld>> repository;
    private Mock<IChatCommand> chatCommand;
    private Mock<IWildcardReplacer> wildcardReplacer;

    private TextCommandOld textCommandOld;

    private TextCommandReplyLoader replyLoader;

    [SetUp]
    public void Setup()
    {
        this.repository = new Mock<IRepository<TextCommandOld>>();
        this.wildcardReplacer = new Mock<IWildcardReplacer>();
        this.chatCommand = new Mock<IChatCommand>();
        this.chatCommand.Setup(c => c.ChatMessage).Returns(new Mock<IChatMessage>().Object);
        this.StubWildcardReplacer();

        this.textCommandOld = new TextCommandOld();

        this.replyLoader = new TextCommandReplyLoader(this.repository.Object, this.wildcardReplacer.Object);
    }

    [TestCase("command1", "command1", "reply1")]
    [TestCase("command2", "CoMMaND2", "reply2")]
    public void TryGetReplyShouldReturnReplyForMatchingCommand(string command, string textCommandText, string replyText)
    {
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns([]);
        this.chatCommand.Setup(c => c.CommandText).Returns(command);
        this.textCommandOld.Commands.Add(textCommandText);
        this.textCommandOld.Replies.Add(replyText);

        bool result = this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.IsTrue(result);
        Assert.AreEqual(replyText, resultText);
    }

    [TestCase("message1 {{user}}", "userName1", "message1 userName1")]
    [TestCase("message2 {{user}}", "userName2", "message2 userName2")]
    public void TryGetReplyShouldReplaceUser(string replyText, string userName, string expectedReply)
    {
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns([]);
        this.chatCommand.Setup(c => c.CommandText).Returns("commandOld");
        this.chatCommand.Setup(c => c.ChatMessage.UserName).Returns(userName);
        this.textCommandOld.Commands.Add("commandOld");
        this.textCommandOld.Replies.Add(replyText);

        bool result = this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.IsTrue(result);
        Assert.AreEqual(expectedReply, resultText);
    }

    [TestCase("parameters1", "text with {{parameters}}", "text with parameters1")]
    [TestCase("parameters2", "other text {{parameters}}", "other text parameters2")]
    public void TryGetReplyShouldReplaceParameterList(string parametersAsString, string commandText, string expected)
    {
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns(["parameters"]);
        this.chatCommand.Setup(c => c.ArgumentsAsString).Returns(parametersAsString);
        this.chatCommand.Setup(c => c.CommandText).Returns("commandOld");
        this.textCommandOld.Replies.Add(commandText);
        this.textCommandOld.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.AreEqual(expected, resultText);
    }

    [Test]
    public void TryGetReplyShouldPrioritizeRepliesWithParameters()
    {
        this.chatCommand.Setup(c => c.ArgumentsAsString).Returns("parameters");
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns(["parameter"]);
        this.chatCommand.Setup(c => c.CommandText).Returns("commandOld");
        this.textCommandOld.Replies.Add("reply");
        this.textCommandOld.Replies.Add("reply with {{parameters}}");
        this.textCommandOld.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.AreEqual("reply with parameters", resultText);
    }

    [Test]
    public void TryGetReplyShouldIgnoreRepliesWithMorePlaceholdersThanActualParameters()
    {
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns(["parameter"]);
        this.chatCommand.Setup(c => c.CommandText).Returns("commandOld");
        this.textCommandOld.Replies.Add("reply with {{parameter2}}");
        this.textCommandOld.Replies.Add("reply with {{parameter3}}");
        this.textCommandOld.Replies.Add("reply");
        this.textCommandOld.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.AreEqual("reply", resultText);
    }

    [Test]
    public void TryGetReplyShouldReturnRegularReplyIfNoReplyContainsParameters()
    {
        this.chatCommand.Setup(c => c.ArgumentsAsString).Returns("parameters");
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns(["parameter"]);
        this.chatCommand.Setup(c => c.CommandText).Returns("commandOld");
        this.textCommandOld.Replies.Add("reply");
        this.textCommandOld.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.AreEqual("reply", resultText);
    }

    [TestCase("text with {{parameter2}} {{parameter1}}", "parameter", "parameter", "text with parameter parameter")]
    [TestCase("text with just one {{parameter2}}", "parameter1", "parameter2", "text with just one parameter2")]
    public void TryGetReplyShouldReplaceAllParameters(string replyText, string parameter1, string parameter2, string expected)
    {
        this.StubWildcardReplacer();
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns([parameter1, parameter2]);
        this.chatCommand.Setup(c => c.CommandText).Returns("commandOld");
        this.textCommandOld.Replies.Add(replyText);
        this.textCommandOld.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.AreEqual(expected, resultText);
    }

    [TestCase("command1", "command2")]
    [TestCase("command2", "command1")]
    public void TryGetReplyShouldReturnFalseWhenMismatchingCommand(string command, string textCommandText)
    {
        this.chatCommand.Setup(c => c.CommandText).Returns(command);
        this.textCommandOld.Commands.Add(textCommandText);

        bool result = this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        Assert.IsFalse(result);
        Assert.IsNull(resultText);
    }

    [Test]
    public void TryGetResponseShouldSetLastUsageDateIfMatchWasFound()
    {
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns([]);
        this.chatCommand.Setup(m => m.CommandText).Returns("commandOld");
        this.textCommandOld.Commands.Add("commandOld");
        this.textCommandOld.Replies.Add("response");

        this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string response);

        Assert.That(this.textCommandOld.LastUsed, Is.EqualTo(DateTime.Now).Within(2).Seconds);
        this.repository.Verify(r => r.Write(this.textCommandOld));
    }

    [Test]
    public void TryGetResponseShouldIncreaseUseCount()
    {
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns([]);
        this.chatCommand.Setup(m => m.CommandText).Returns("commandOld");
        this.textCommandOld.Commands.Add("commandOld");
        this.textCommandOld.Replies.Add("response");

        this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string response);

        Assert.AreEqual(1, this.textCommandOld.UseCount);
    }

    [TestCase(10)]
    [TestCase(20)]
    public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
    {
        this.textCommandOld.LastUsed = DateTime.Now.AddSeconds(-timeout + 1);
        this.textCommandOld.TimeoutInSeconds = timeout;

        bool result = this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string response);

        Assert.IsNull(response);
        Assert.IsFalse(result);
    }

    [Test]
    public void TryGetResponseShouldUseWildCardReplacer()
    {
        this.chatCommand.Setup(c => c.ArgumentsAsList).Returns([]);
        this.chatCommand.Setup(c => c.CommandText).Returns("commandOld");
        this.textCommandOld.Commands.Add("commandOld");
        this.textCommandOld.Replies.Add("reply");

        bool result = this.replyLoader.TryGetReply(this.textCommandOld, this.chatCommand.Object, out string resultText);

        this.wildcardReplacer.Verify(r => r.Replace("reply"));
    }

    private void StubWildcardReplacer()
    {
        this.wildcardReplacer.Setup(r => r.Replace(It.IsAny<string>())).Returns<string>(s => s);
    }
}