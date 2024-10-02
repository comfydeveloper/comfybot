using ComfyBot.Bot.ChatBot.Messages;
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

namespace ComfyBot.Bot.Tests.ChatBot.Messages;

[TestFixture]
public class MessageResponseHandlerTests
{
    private IMessageResponseLoader responseLoader;
    private ITwitchClient twitchClient;
    private IQueryableRepository repository;
    private IChatMessage chatMessage;
    private BotSettings settings;

    private MessageResponseHandler handler;

    [SetUp]
    public void Setup()
    {
        this.repository = Substitute.For<IQueryableRepository>();
        this.twitchClient = Substitute.For<ITwitchClient>();
        this.responseLoader = Substitute.For<IMessageResponseLoader>();
        this.chatMessage = Substitute.For<IChatMessage>();

        this.settings = new BotSettings();
        IOptions<BotSettings> options = Substitute.For<IOptions<BotSettings>>();
        options.Value.Returns(this.settings);

        this.handler = new MessageResponseHandler(this.repository, this.responseLoader, options);
    }

    [TestCase("channel1", "response1")]
    [TestCase("channel2", "response2")]
    public void HandleShouldSendMessageIfSuitableMessageFound(string channel, string response)
    {
        this.settings.Channel = channel;
        MessageResponse messageResponse1 = CreateMessageResponse();
        MessageResponse messageResponse2 = CreateMessageResponse();
        this.chatMessage.Text.Returns("message");
        this.repository.Query<MessageResponse>().Returns(new[] { messageResponse1, messageResponse2, messageResponse2 }.AsQueryable());
        this.responseLoader.TryGetResponse(messageResponse1, this.chatMessage, out Arg.Any<string>()).Returns(false);
        this.responseLoader.TryGetResponse(messageResponse2, this.chatMessage, out Arg.Any<string>()).Returns(true);

        this.handler.Handle(this.twitchClient, this.chatMessage);

        this.responseLoader.Received(1).TryGetResponse(messageResponse1, this.chatMessage, out Arg.Any<string>());
        this.responseLoader.TryGetResponse(messageResponse2, this.chatMessage, out Arg.Any<string>());
        this.twitchClient.SendMessage(channel, response, false);
    }

    private static MessageResponse CreateMessageResponse(int priority = 0)
    {
        return new MessageResponse
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Users = [],
            LooseKeywords = [],
            AllKeywords = [],
            ExactKeywords = [],
            Replies = [],
            LastUsedAt = DateTime.Now,
            TimeoutInSeconds = 0,
            UseCount = 0,
            Priority = priority,
            AlwaysReply = false
        };
    }

    [Test]
    public void HandleShouldSendSuitableMessageOrderedByPriority()
    {
        string response1 = "response1";
        string response2 = "response2";
        this.settings.Channel = "channel";
        MessageResponse messageResponse1 = CreateMessageResponse(priority: 2);
        MessageResponse messageResponse2 = CreateMessageResponse(priority: 1);
        this.chatMessage.Text.Returns("message");
        this.repository.Query<MessageResponse>().Returns(new[] { messageResponse1, messageResponse2 }.AsQueryable());
        this.responseLoader.TryGetResponse(messageResponse1, this.chatMessage, out Arg.Any<string>()).Returns(x =>
        {
            x[2] = response1;
            return true;
        });
        this.responseLoader.TryGetResponse(messageResponse2, this.chatMessage, out Arg.Any<string>()).Returns(x =>
        {
            x[2] = response2;
            return true;
        });

        this.handler.Handle(this.twitchClient, this.chatMessage);

        this.twitchClient.Received(1).SendMessage("channel", response2, false);
    }

    [TestCase("!")]
    [TestCase("!test")]
    [TestCase("! test")]
    public void HandleShouldNotSendResponseWhenMessageIsCommand(string commandMessage)
    {
        this.chatMessage.Text.Returns(commandMessage);

        this.handler.Handle(this.twitchClient, this.chatMessage);

        this.repository.DidNotReceive().Query<MessageResponse>();
    }
}