namespace ComfyBot.Bot.Tests.ChatBot.Messages
{
    using ComfyBot.Bot.ChatBot.Messages;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using Data.Models;
    using Data.Repositories;
    using Settings;

    using Moq;

    using NUnit.Framework;

    using TwitchLib.Client.Interfaces;

    [TestFixture]
    public class MessageResponseHandlerTests
    {
        private Mock<IRepository<MessageResponse>> repository;
        private Mock<IMessageResponseLoader> responseLoader;
        private Mock<ITwitchClient> twitchClient;
        private Mock<IChatMessage> chatMessage;

        private MessageResponseHandler handler;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<IRepository<MessageResponse>>();
            twitchClient = new Mock<ITwitchClient>();
            responseLoader = new Mock<IMessageResponseLoader>();
            chatMessage = new Mock<IChatMessage>();

            handler = new MessageResponseHandler(repository.Object, responseLoader.Object);
        }

        [TestCase("channel1", "response1")]
        [TestCase("channel2", "response2")]
        public void HandleShouldSendMessageIfSuitableMessageFound(string channel, string response)
        {
            ApplicationSettings.Default.Channel = channel;
            MessageResponse messageResponse1 = new MessageResponse();
            MessageResponse messageResponse2 = new MessageResponse();
            chatMessage.Setup(m => m.Text).Returns("message");
            repository.Setup(r => r.GetAll()).Returns(new[] { messageResponse1, messageResponse2, messageResponse2 });
            responseLoader.Setup(r => r.TryGetResponse(messageResponse1, chatMessage.Object, out response)).Returns(false);
            responseLoader.Setup(r => r.TryGetResponse(messageResponse2, chatMessage.Object, out response)).Returns(true);

            handler.Handle(twitchClient.Object, chatMessage.Object);

            responseLoader.Verify(r => r.TryGetResponse(messageResponse1, chatMessage.Object, out response));
            responseLoader.Verify(r => r.TryGetResponse(messageResponse2, chatMessage.Object, out response));
            twitchClient.Verify(c => c.SendMessage(channel, response, false), Times.Once);
        }

        [Test]
        public void HandleShouldSendSuitableMessageOrderedByPriority()
        {
            string response1 = "response1";
            string response2 = "response2";
            ApplicationSettings.Default.Channel = "channel";
            MessageResponse messageResponse1 = new MessageResponse { Priority = 2 };
            MessageResponse messageResponse2 = new MessageResponse { Priority = 1 };
            chatMessage.Setup(m => m.Text).Returns("message");
            repository.Setup(r => r.GetAll()).Returns(new[] { messageResponse1, messageResponse2 });
            responseLoader.Setup(r => r.TryGetResponse(messageResponse1, chatMessage.Object, out response1)).Returns(true);
            responseLoader.Setup(r => r.TryGetResponse(messageResponse2, chatMessage.Object, out response2)).Returns(true);

            handler.Handle(twitchClient.Object, chatMessage.Object);

            twitchClient.Verify(c => c.SendMessage("channel", response2, false), Times.Once);
        }

        [TestCase("!")]
        [TestCase("!test")]
        [TestCase("! test")]
        public void HandleShouldNotSendResponseWhenMessageIsCommand(string commandMessage)
        {
            chatMessage.Setup(m => m.Text).Returns(commandMessage);

            handler.Handle(twitchClient.Object, chatMessage.Object);

            repository.Verify(r => r.GetAll(), Times.Never);
        }
    }
}