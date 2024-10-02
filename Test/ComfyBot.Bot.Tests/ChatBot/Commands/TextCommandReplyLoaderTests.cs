using System;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using NSubstitute;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TextCommandReplyLoaderTests
{
    private IChatCommand chatCommand;
    private IWildcardReplacer wildcardReplacer;

    private TextCommand textCommand;

    private TextCommandReplyLoader replyLoader;

    [SetUp]
    public void Setup()
    {
        this.wildcardReplacer = Substitute.For<IWildcardReplacer>();
        this.chatCommand = Substitute.For<IChatCommand>();
        this.chatCommand.ChatMessage.Returns(Substitute.For<IChatMessage>());
        this.StubWildcardReplacer();

        this.textCommand = new TextCommand
        {
            Replies = [],
            Commands = [],
            LastUsedAt = DateTime.Now,
            UseCount = 0,
            TimeoutInSeconds = 0,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now
        };

        this.replyLoader = new TextCommandReplyLoader(this.wildcardReplacer);
    }

    [TestCase("command1", "command1", "reply1")]
    [TestCase("command2", "CoMMaND2", "reply2")]
    public void TryGetReplyShouldReturnReplyForMatchingCommand(string command, string textCommandText, string replyText)
    {
        this.chatCommand.ArgumentsAsList.Returns([]);
        this.chatCommand.CommandText.Returns(command);
        this.textCommand.Commands.Add(textCommandText);
        this.textCommand.Replies.Add(replyText);

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.IsTrue(result);
        Assert.AreEqual(replyText, resultText);
    }

    [TestCase("message1 {{user}}", "userName1", "message1 userName1")]
    [TestCase("message2 {{user}}", "userName2", "message2 userName2")]
    public void TryGetReplyShouldReplaceUser(string replyText, string userName, string expectedReply)
    {
        this.chatCommand.ArgumentsAsList.Returns([]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.chatCommand.ChatMessage.UserName.Returns(userName);
        this.textCommand.Commands.Add("commandOld");
        this.textCommand.Replies.Add(replyText);

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.IsTrue(result);
        Assert.AreEqual(expectedReply, resultText);
    }

    [TestCase("parameters1", "text with {{parameters}}", "text with parameters1")]
    [TestCase("parameters2", "other text {{parameters}}", "other text parameters2")]
    public void TryGetReplyShouldReplaceParameterList(string parametersAsString, string commandText, string expected)
    {
        this.chatCommand.ArgumentsAsList.Returns(["parameters"]);
        this.chatCommand.ArgumentsAsString.Returns(parametersAsString);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Replies.Add(commandText);
        this.textCommand.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.AreEqual(expected, resultText);
    }

    [Test]
    public void TryGetReplyShouldPrioritizeRepliesWithParameters()
    {
        this.chatCommand.ArgumentsAsString.Returns("parameters");
        this.chatCommand.ArgumentsAsList.Returns(["parameter"]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Replies.Add("reply");
        this.textCommand.Replies.Add("reply with {{parameters}}");
        this.textCommand.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.AreEqual("reply with parameters", resultText);
    }

    [Test]
    public void TryGetReplyShouldIgnoreRepliesWithMorePlaceholdersThanActualParameters()
    {
        this.chatCommand.ArgumentsAsList.Returns(["parameter"]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Replies.Add("reply with {{parameter2}}");
        this.textCommand.Replies.Add("reply with {{parameter3}}");
        this.textCommand.Replies.Add("reply");
        this.textCommand.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.AreEqual("reply", resultText);
    }

    [Test]
    public void TryGetReplyShouldReturnRegularReplyIfNoReplyContainsParameters()
    {
        this.chatCommand.ArgumentsAsString.Returns("parameters");
        this.chatCommand.ArgumentsAsList.Returns(["parameter"]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Replies.Add("reply");
        this.textCommand.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.AreEqual("reply", resultText);
    }

    [TestCase("text with {{parameter2}} {{parameter1}}", "parameter", "parameter", "text with parameter parameter")]
    [TestCase("text with just one {{parameter2}}", "parameter1", "parameter2", "text with just one parameter2")]
    public void TryGetReplyShouldReplaceAllParameters(string replyText, string parameter1, string parameter2, string expected)
    {
        this.StubWildcardReplacer();
        this.chatCommand.ArgumentsAsList.Returns([parameter1, parameter2]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Replies.Add(replyText);
        this.textCommand.Commands.Add("commandOld");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.AreEqual(expected, resultText);
    }

    [TestCase("command1", "command2")]
    [TestCase("command2", "command1")]
    public void TryGetReplyShouldReturnFalseWhenMismatchingCommand(string command, string textCommandText)
    {
        this.chatCommand.CommandText.Returns(command);
        this.textCommand.Commands.Add(textCommandText);

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        Assert.IsFalse(result);
        Assert.IsNull(resultText);
    }

    [Test]
    public void TryGetResponseShouldIncreaseUseCount()
    {
        this.chatCommand.ArgumentsAsList.Returns([]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Commands.Add("commandOld");
        this.textCommand.Replies.Add("response");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string response);

        Assert.AreEqual(1, this.textCommand.UseCount);
    }

    [TestCase(10)]
    [TestCase(20)]
    public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
    {
        this.textCommand.LastUsedAt = DateTime.Now.AddSeconds(-timeout + 1);
        this.textCommand.TimeoutInSeconds = timeout;

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string response);

        Assert.IsNull(response);
        Assert.IsFalse(result);
    }

    [Test]
    public void TryGetResponseShouldUseWildCardReplacer()
    {
        this.chatCommand.ArgumentsAsList.Returns([]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Commands.Add("commandOld");
        this.textCommand.Replies.Add("reply");

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        this.wildcardReplacer.Received(1).Replace("reply");
    }

    private void StubWildcardReplacer()
    {
        this.wildcardReplacer.Replace(default).ReturnsForAnyArgs(s => (string)s[0]);
    }
}