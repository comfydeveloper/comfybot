using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TextCommandHandlerTests
{
    private IQueryableRepository repository;
    private ITextCommandReplyLoader replyLoader;

    private IChatCommand chatCommand;
    private IMessageSender messageSender;

    private TextCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        this.repository = Substitute.For<IQueryableRepository>();
        this.replyLoader = Substitute.For<ITextCommandReplyLoader>();

        this.chatCommand = Substitute.For<IChatCommand>();
        this.messageSender = Substitute.For<IMessageSender>();

        this.handler = new TextCommandHandler(this.repository, this.replyLoader, this.messageSender);
    }

    [TestCase("channel1", "reply1")]
    [TestCase("channel2", "reply2")]
    public void HandleShouldSendLoadedReply(string channel, string reply)
    {
        TextCommand command1 = CreateCommand();
        TextCommand command2 = CreateCommand();
        this.repository.Query<TextCommand>().Returns(new[] { command1, command2 }.AsQueryable());
        this.replyLoader.TryGetReply(command1, this.chatCommand, out Arg.Any<string>()).Returns(false);
        this.replyLoader.TryGetReply(command2, this.chatCommand, out Arg.Any<string>()).Returns(x =>
        {
            x[2] = reply;
            return true;
        });

        this.handler.Handle(this.chatCommand);

        this.messageSender.Received(1).Send(reply);
    }

    [Test]
    public void HandleShouldSendNothingIfNoReplyFound()
    {
        TextCommand command = CreateCommand();
        this.repository.Query<TextCommand>().Returns(new[] { command }.AsQueryable());
        this.replyLoader.TryGetReply(command, this.chatCommand, out Arg.Any<string>()).Returns(false);

        this.handler.Handle(this.chatCommand);

        this.messageSender.DidNotReceive().Send(Arg.Any<string>());
    }

    private static TextCommand CreateCommand()
    {
        return new TextCommand
        {
            Replies = [],
            Commands = [],
            LastUsedAt = DateTime.UtcNow,
            UseCount = 0,
            TimeoutInSeconds = 0,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
