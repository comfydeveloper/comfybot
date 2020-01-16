namespace ComfyBot.Bot.Tests.ChatBot.Messages
{
    using ComfyBot.Bot.ChatBot.Messages;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;
    using ComfyBot.Settings;

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
            this.repository = new Mock<IRepository<MessageResponse>>();
            this.twitchClient = new Mock<ITwitchClient>();
            this.responseLoader = new Mock<IMessageResponseLoader>();
            this.chatMessage = new Mock<IChatMessage>();

            this.handler = new MessageResponseHandler(this.repository.Object, this.responseLoader.Object);
        }

        [TestCase("channel1", "response1")]
        [TestCase("channel2", "response2")]
        public void HandleShouldSendMessageIfSuitableMessageFound(string channel, string response)
        {
            ApplicationSettings.Default.Channel = channel;
            MessageResponse messageResponse1 = new MessageResponse();
            MessageResponse messageResponse2 = new MessageResponse();
            this.repository.Setup(r => r.GetAll()).Returns(new[] { messageResponse1, messageResponse2, messageResponse2 });
            this.responseLoader.Setup(r => r.TryGetResponse(messageResponse1, this.chatMessage.Object, out response)).Returns(false);
            this.responseLoader.Setup(r => r.TryGetResponse(messageResponse2, this.chatMessage.Object, out response)).Returns(true);

            this.handler.Handle(this.twitchClient.Object, this.chatMessage.Object);

            this.responseLoader.Verify(r => r.TryGetResponse(messageResponse1, this.chatMessage.Object, out response));
            this.responseLoader.Verify(r => r.TryGetResponse(messageResponse2, this.chatMessage.Object, out response));
            this.twitchClient.Verify(c => c.SendMessage(channel, response, false), Times.Once);
        }
    }
}