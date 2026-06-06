using System;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Data.Models;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TextCommandReplyLoaderTests
{
    private IChatCommand chatCommand;
    private IWildcardReplacer wildcardReplacer;

    private TextCommand textCommand;

    private TextCommandReplyLoader replyLoader;

    private ChatMessage chatMessage;

    [SetUp]
    public void Setup()
    {
        this.wildcardReplacer = Substitute.For<IWildcardReplacer>();
        this.chatCommand = Substitute.For<IChatCommand>();
        this.chatMessage = new ChatMessage();
        this.chatCommand.ChatMessage.Returns(this.chatMessage);
        this.StubWildcardReplacer();

        this.textCommand = new TextCommand
        {
            Replies = [],
            Commands = [],
            LastUsedAt = DateTime.UtcNow,
            UseCount = 0,
            TimeoutInSeconds = 0,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
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

        result.ShouldBeTrue();
        resultText.ShouldBe(replyText);
    }

    [TestCase("message1 {{user}}", "userName1", "message1 userName1")]
    [TestCase("message2 {{user}}", "userName2", "message2 userName2")]
    public void TryGetReplyShouldReplaceUser(string replyText, string userName, string expectedReply)
    {
        this.chatCommand.ArgumentsAsList.Returns([]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.chatMessage.UserName = userName;
        this.textCommand.Commands.Add("commandOld");
        this.textCommand.Replies.Add(replyText);

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        result.ShouldBeTrue();
        resultText.ShouldBe(expectedReply);
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

        resultText.ShouldBe(expected);
    }

    [Test]
    public void TryGetReplyShouldPrioritizeRepliesWithParameters()
    {
        this.chatCommand.ArgumentsAsString.Returns("parameters");
        this.chatCommand.ArgumentsAsList.Returns(["parameter"]);
        this.chatCommand.CommandText.Returns("command");
        this.textCommand.Replies.Add("reply");
        this.textCommand.Replies.Add("reply with {{parameters}}");
        this.textCommand.Commands.Add("command");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        resultText.ShouldBe("reply with parameters");
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

        resultText.ShouldBe("reply");
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

        resultText.ShouldBe("reply");
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

        resultText.ShouldBe(expected);
    }

    [TestCase("command1", "command2")]
    [TestCase("command2", "command1")]
    public void TryGetReplyShouldReturnFalseWhenMismatchingCommand(string command, string textCommandText)
    {
        this.chatCommand.CommandText.Returns(command);
        this.textCommand.Commands.Add(textCommandText);

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string resultText);

        result.ShouldBeFalse();
        resultText.ShouldBeNull();
    }

    [TestCase(10)]
    [TestCase(20)]
    public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
    {
        this.textCommand.LastUsedAt = DateTime.UtcNow.AddSeconds(-timeout + 1);
        this.textCommand.TimeoutInSeconds = timeout;

        bool result = this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string response);

        response.ShouldBeNull();
        result.ShouldBeFalse();
    }

    [Test]
    public void TryGetResponseShouldUseWildCardReplacer()
    {
        this.chatCommand.ArgumentsAsList.Returns([]);
        this.chatCommand.CommandText.Returns("commandOld");
        this.textCommand.Commands.Add("commandOld");
        this.textCommand.Replies.Add("reply");

        this.replyLoader.TryGetReply(this.textCommand, this.chatCommand, out string _);

        this.wildcardReplacer.Received(1).Replace("reply");
    }

    private void StubWildcardReplacer()
    {
        this.wildcardReplacer.Replace(default, default).ReturnsForAnyArgs(s => (string)s[0]);
        this.wildcardReplacer.Replace(default).ReturnsForAnyArgs(s => (string)s[0]);
    }
}
