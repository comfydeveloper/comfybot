using System;
using System.Collections.Generic;
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
    private Mock<IRepository<TextCommand>> repository;
    private Mock<IChatCommand> chatCommand;
    private Mock<IWildcardReplacer> wildcardReplacer;

    private TextCommand textCommand;

    private TextCommandReplyLoader replyLoader;

    [SetUp]
    public void Setup()
    {
        repository = new Mock<IRepository<TextCommand>>();
        wildcardReplacer = new Mock<IWildcardReplacer>();
        chatCommand = new Mock<IChatCommand>();
        chatCommand.Setup(c => c.ChatMessage).Returns(new Mock<IChatMessage>().Object);
        StubWildcardReplacer();

        textCommand = new TextCommand();

        replyLoader = new TextCommandReplyLoader(repository.Object, wildcardReplacer.Object);
    }

    [TestCase("command1", "command1", "reply1")]
    [TestCase("command2", "CoMMaND2", "reply2")]
    public void TryGetReplyShouldReturnReplyForMatchingCommand(string command, string textCommandText, string replyText)
    {
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string>());
        chatCommand.Setup(c => c.CommandText).Returns(command);
        textCommand.Commands.Add(textCommandText);
        textCommand.Replies.Add(replyText);

        bool result = replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.IsTrue(result);
        Assert.AreEqual(replyText, resultText);
    }

    [TestCase("message1 {{user}}", "userName1", "message1 userName1")]
    [TestCase("message2 {{user}}", "userName2", "message2 userName2")]
    public void TryGetReplyShouldReplaceUser(string replyText, string userName, string expectedReply)
    {
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string>());
        chatCommand.Setup(c => c.CommandText).Returns("command");
        chatCommand.Setup(c => c.ChatMessage.UserName).Returns(userName);
        textCommand.Commands.Add("command");
        textCommand.Replies.Add(replyText);

        bool result = replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.IsTrue(result);
        Assert.AreEqual(expectedReply, resultText);
    }

    [TestCase("parameters1", "text with {{parameters}}", "text with parameters1")]
    [TestCase("parameters2", "other text {{parameters}}", "other text parameters2")]
    public void TryGetReplyShouldReplaceParameterList(string parametersAsString, string commandText, string expected)
    {
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { "parameters" });
        chatCommand.Setup(c => c.ArgumentsAsString).Returns(parametersAsString);
        chatCommand.Setup(c => c.CommandText).Returns("command");
        textCommand.Replies.Add(commandText);
        textCommand.Commands.Add("command");

        replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.AreEqual(expected, resultText);
    }

    [Test]
    public void TryGetReplyShouldPriorizeRepliesWithParameters()
    {
        chatCommand.Setup(c => c.ArgumentsAsString).Returns("parameters");
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> {"parameter" });
        chatCommand.Setup(c => c.CommandText).Returns("command");
        textCommand.Replies.Add("reply");
        textCommand.Replies.Add("reply with {{parameters}}");
        textCommand.Commands.Add("command");

        replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.AreEqual("reply with parameters", resultText);
    }

    [Test]
    public void TryGetReplyShouldIgnoreRepliesWithMorePlaceholdersThanActualParameters()
    {
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { "parameter" });
        chatCommand.Setup(c => c.CommandText).Returns("command");
        textCommand.Replies.Add("reply with {{parameter2}}");
        textCommand.Replies.Add("reply with {{parameter3}}");
        textCommand.Replies.Add("reply");
        textCommand.Commands.Add("command");

        replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.AreEqual("reply", resultText);
    }

    [Test]
    public void TryGetReplyShouldReturnRegularReplyIfNoReplyContainsParameters()
    {
        chatCommand.Setup(c => c.ArgumentsAsString).Returns("parameters");
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { "parameter" });
        chatCommand.Setup(c => c.CommandText).Returns("command");
        textCommand.Replies.Add("reply");
        textCommand.Commands.Add("command");

        replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.AreEqual("reply", resultText);
    }

    [TestCase("text with {{parameter2}} {{parameter1}}", "parameter", "parameter", "text with parameter parameter")]
    [TestCase("text with just one {{parameter2}}", "parameter1", "parameter2", "text with just one parameter2")]
    public void TryGetReplyShouldReplaceAllParameters(string replyText, string parameter1, string parameter2, string expected)
    {
        StubWildcardReplacer();
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string> { parameter1, parameter2 });
        chatCommand.Setup(c => c.CommandText).Returns("command");
        textCommand.Replies.Add(replyText);
        textCommand.Commands.Add("command");

        replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.AreEqual(expected, resultText);
    }

    [TestCase("command1", "command2")]
    [TestCase("command2", "command1")]
    public void TryGetReplyShouldReturnFalseWhenMismatchingCommand(string command, string textCommandText)
    {
        chatCommand.Setup(c => c.CommandText).Returns(command);
        textCommand.Commands.Add(textCommandText);

        bool result = replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        Assert.IsFalse(result);
        Assert.IsNull(resultText);
    }

    [Test]
    public void TryGetResponseShouldSetLastUsageDateIfMatchWasFound()
    {
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string>());
        chatCommand.Setup(m => m.CommandText).Returns("command");
        textCommand.Commands.Add("command");
        textCommand.Replies.Add("response");

        replyLoader.TryGetReply(textCommand, chatCommand.Object, out string response);

        Assert.That(textCommand.LastUsed, Is.EqualTo(DateTime.Now).Within(2).Seconds);
        repository.Verify(r => r.Write(textCommand));
    }

    [Test]
    public void TryGetResponseShouldIncreaseUseCount()
    {
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string>());
        chatCommand.Setup(m => m.CommandText).Returns("command");
        textCommand.Commands.Add("command");
        textCommand.Replies.Add("response");

        replyLoader.TryGetReply(textCommand, chatCommand.Object, out string response);

        Assert.AreEqual(1, textCommand.UseCount);
    }

    [TestCase(10)]
    [TestCase(20)]
    public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
    {
        textCommand.LastUsed = DateTime.Now.AddSeconds(-timeout + 1);
        textCommand.TimeoutInSeconds = timeout;

        bool result = replyLoader.TryGetReply(textCommand, chatCommand.Object, out string response);

        Assert.IsNull(response);
        Assert.IsFalse(result);
    }

    [Test]
    public void TryGetResponseShouldUseWildCardReplacer()
    {
        chatCommand.Setup(c => c.ArgumentsAsList).Returns(new List<string>());
        chatCommand.Setup(c => c.CommandText).Returns("command");
        textCommand.Commands.Add("command");
        textCommand.Replies.Add("reply");

        bool result = replyLoader.TryGetReply(textCommand, chatCommand.Object, out string resultText);

        wildcardReplacer.Verify(r => r.Replace("reply"));
    }

    private void StubWildcardReplacer()
    {
        wildcardReplacer.Setup(r => r.Replace(It.IsAny<string>())).Returns<string>(s => s);
    }
}