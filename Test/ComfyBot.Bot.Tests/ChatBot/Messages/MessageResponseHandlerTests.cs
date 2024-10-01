using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using NUnit.Framework;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Tests.ChatBot.Messages;

[TestFixture]
public class MessageResponseHandlerTests
{
    private Mock<IRepository<MessageResponseOld>> repository;
    private Mock<IMessageResponseLoader> responseLoader;
    private Mock<ITwitchClient> twitchClient;
    private Mock<IChatMessage> chatMessage;

    private MessageResponseHandler handler;

    [SetUp]
    public void Setup()
    {
        this.repository = Substitute.For<IRepository<MessageResponseOld>>();
        this.twitchClient = Substitute.For<ITwitchClient>();
        this.responseLoader = Substitute.For<IMessageResponseLoader>();
        this.chatMessage = Substitute.For<IChatMessage>();

        this.handler = new MessageResponseHandler(this.repository, this.responseLoader);
    }

    [TestCase("channel1", "response1")]
    [TestCase("channel2", "response2")]
    public void HandleShouldSendMessageIfSuitableMessageFound(string channel, string response)
    {
        this.settings.Channel = channel;
        MessageResponseOld messageResponse1 = new();
        MessageResponseOld messageResponse2 = new();
        this.chatMessage.Setup(m => m.Text).Returns("message");
        this.repository.Setup(r => r.GetAll()).Returns(new[] { messageResponse1, messageResponse2, messageResponse2 });
        this.responseLoader.Setup(r => r.TryGetResponse(messageResponse1, this.chatMessage, out response)).Returns(false);
        this.responseLoader.Setup(r => r.TryGetResponse(messageResponse2, this.chatMessage, out response)).Returns(true);

        this.handler.Handle(this.twitchClient, this.chatMessage);

        this.responseLoader.Verify(r => r.TryGetResponse(messageResponse1, this.chatMessage, out response));
        this.responseLoader.Verify(r => r.TryGetResponse(messageResponse2, this.chatMessage, out response));
        this.twitchClient.Verify(c => c.SendMessage(channel, response, false), Times.Once);
    }

    [Test]
    public void HandleShouldSendSuitableMessageOrderedByPriority()
    {
        string response1 = "response1";
        string response2 = "response2";
        this.settings.Channel = "channel";
        MessageResponseOld messageResponse1 = new() { Priority = 2 };
        MessageResponseOld messageResponse2 = new() { Priority = 1 };
        this.chatMessage.Setup(m => m.Text).Returns("message");
        this.repository.Setup(r => r.GetAll()).Returns(new[] { messageResponse1, messageResponse2 });
        this.responseLoader.Setup(r => r.TryGetResponse(messageResponse1, this.chatMessage, out response1)).Returns(true);
        this.responseLoader.Setup(r => r.TryGetResponse(messageResponse2, this.chatMessage, out response2)).Returns(true);

        this.handler.Handle(this.twitchClient, this.chatMessage);

        this.twitchClient.Verify(c => c.SendMessage("channel", response2, false), Times.Once);
    }

    [TestCase("!")]
    [TestCase("!test")]
    [TestCase("! test")]
    public void HandleShouldNotSendResponseWhenMessageIsCommand(string commandMessage)
    {
        this.chatMessage.Setup(m => m.Text).Returns(commandMessage);

        this.handler.Handle(this.twitchClient, this.chatMessage);

        this.repository.Verify(r => r.GetAll(), Times.Never);
    }
}