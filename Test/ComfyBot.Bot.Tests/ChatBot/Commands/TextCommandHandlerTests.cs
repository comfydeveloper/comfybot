using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Scaffolding;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot.Commands;

[TestFixture]
public class TextCommandHandlerTests
{
    private IQueryableRepository repository;
    private ITextCommandReplyLoader replyLoader;

    private ITwitchClient twitchClient;
    private IChatCommand chatCommand;
    private BotSettings settings;

    private TextCommandHandler handler;

    [SetUp]
    public void Setup()
    {
        this.repository = Substitute.For<IQueryableRepository>();
        this.replyLoader = Substitute.For<ITextCommandReplyLoader>();

        this.twitchClient = Substitute.For<ITwitchClient>();
        this.chatCommand = Substitute.For<IChatCommand>();

        this.settings = new BotSettings();
        IOptions<BotSettings> options = Substitute.For<IOptions<BotSettings>>();
        options.Value.Returns(this.settings);

        this.handler = new TextCommandHandler(this.repository, this.replyLoader, options);
    }

    [TestCase("channel1", "reply1")]
    [TestCase("channel2", "reply2")]
    public void HandleShouldSendLoadedReply(string channel, string reply)
    {
        this.settings.Channel = channel;
        TextCommand command1 = CreateCommand();
        TextCommand command2 = CreateCommand();
        this.repository.Query<TextCommand>().Returns(new[] { command1, command2 }.AsQueryable());
        this.replyLoader.TryGetReply(command1, this.chatCommand, out Arg.Any<string>()).Returns(false);
        this.replyLoader.TryGetReply(command2, this.chatCommand, out Arg.Any<string>()).Returns(x =>
        {
            x[2] = reply;
            return true;
        });

        this.handler.Handle(this.twitchClient, this.chatCommand);

        this.twitchClient.Received(1).SendMessage(channel, reply, false);
    }

    [Test]
    public void HandleShouldSendNothingIfNoReplyFound()
    {
        TextCommand command = CreateCommand();
        this.repository.Query<TextCommand>().Returns(new[] { command }.AsQueryable());
        this.replyLoader.TryGetReply(command, this.chatCommand, out Arg.Any<string>()).Returns(false);

        this.handler.Handle(this.twitchClient, this.chatCommand);

        this.twitchClient.DidNotReceive().SendMessage(Arg.Any<string>(), Arg.Any<string>(), false);
    }

    private static TextCommand CreateCommand()
    {
        return new TextCommand
        {
            Replies = [],
            Commands = [],
            LastUsedAt = DateTime.Now,
            UseCount = 0,
            TimeoutInSeconds = 0,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now
        };
    }
}