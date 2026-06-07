using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Bot.Scaffolding;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using ComfyBot.Bot.Tests.TestHelpers;

namespace ComfyBot.Bot.Tests.ChatBot.Messages;

[TestFixture]
public class ChatMessageResponseHandlerTests
{
    private IMessageResponseLoader responseLoader;
    private IQueryableRepository repository;
    private IChatMessage chatMessage;
    private IMessageSender messageSender;
    private BotSettings settings;

    private ChatMessageResponseHandler handler;

    [SetUp]
    public void Setup()
    {
        this.repository = Substitute.For<IQueryableRepository>();
        this.responseLoader = Substitute.For<IMessageResponseLoader>();
        this.chatMessage = Substitute.For<IChatMessage>();
        this.messageSender = Substitute.For<IMessageSender>();
        this.settings = new BotSettings();
        IOptions<BotSettings> options = Substitute.For<IOptions<BotSettings>>();
        options.Value.Returns(this.settings);

        this.handler = new ChatMessageResponseHandler(this.repository, this.responseLoader, this.messageSender, options);
    }

    [TestCase("channel1", "response1")]
    [TestCase("channel2", "response2")]
    public async Task HandleShouldSendIfSuitableMessageFound(string channel, string response)
    {
        MessageResponse messageResponse1 = CreateMessageResponse();
        MessageResponse messageResponse2 = CreateMessageResponse();
        this.chatMessage.Text.Returns("message");
        this.repository.Query<MessageResponse>().Returns(new TestAsyncEnumerable<MessageResponse>(new[] { messageResponse1, messageResponse2, messageResponse2 }));
        this.responseLoader.TryGetResponse(messageResponse1, this.chatMessage, out Arg.Any<string>()).Returns(false);
        this.responseLoader.TryGetResponse(messageResponse2, this.chatMessage, out Arg.Any<string>()).Returns(true);

        await this.handler.Handle(this.chatMessage);

        this.responseLoader.Received(1).TryGetResponse(messageResponse1, this.chatMessage, out Arg.Any<string>());
        this.responseLoader.TryGetResponse(messageResponse2, this.chatMessage, out Arg.Any<string>());
        this.messageSender.Send(response);
    }

    [Test]
    public async Task HandleShouldSendSuitableMessageOrderedByPriority()
    {
        string response1 = "response1";
        string response2 = "response2";
        MessageResponse messageResponse1 = CreateMessageResponse(priority: 2);
        MessageResponse messageResponse2 = CreateMessageResponse(priority: 1);
        this.chatMessage.Text.Returns("message");
        this.repository.Query<MessageResponse>().Returns(new TestAsyncEnumerable<MessageResponse>(new[] { messageResponse1, messageResponse2 }));
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

        await this.handler.Handle(this.chatMessage);

        this.messageSender.Received(1).Send(response2);
    }

    [TestCase("!")]
    [TestCase("!test")]
    [TestCase("! test")]
    public async Task HandleShouldNotSendResponseWhenMessageIsCommand(string commandMessage)
    {
        this.chatMessage.Text.Returns(commandMessage);

        await this.handler.Handle(this.chatMessage);

        this.repository.DidNotReceive().Query<MessageResponse>();
    }

    [Test]
    public async Task HandleShouldNotSendResponseWhenMessageIsSentByIgnoredUser()
    {
        this.chatMessage.UserName.Returns("ignored");
        this.settings.IgnoredUsers = ["Ignored"];

        await this.handler.Handle(this.chatMessage);

        this.repository.DidNotReceive().Query<MessageResponse>();
    }

    private static MessageResponse CreateMessageResponse(int priority = 0)
    {
        return new MessageResponse
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Users = [],
            LooseKeywords = [],
            AllKeywords = [],
            ExactKeywords = [],
            Replies = [],
            LastUsedAt = DateTime.UtcNow,
            TimeoutInSeconds = 0,
            UseCount = 0,
            Priority = priority,
            AlwaysReply = false
        };
    }
}
