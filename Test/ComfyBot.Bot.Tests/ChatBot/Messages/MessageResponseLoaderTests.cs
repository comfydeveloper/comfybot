namespace ComfyBot.Bot.Tests.ChatBot.Messages
{
    using System;

    using ComfyBot.Bot.ChatBot.Messages;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class MessageResponseLoaderTests
    {
        private Mock<IRepository<MessageResponse>> repository;
        private Mock<IChatMessage> chatMessage;

        private MessageResponse messageResponse;

        private MessageResponseLoader loader;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<IRepository<MessageResponse>>();
            this.chatMessage = new Mock<IChatMessage>();

            this.messageResponse = new MessageResponse();

            this.loader = new MessageResponseLoader(this.repository.Object);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
        {
            this.messageResponse.LastUsed = DateTime.Now.AddSeconds(-timeout + 1);
            this.messageResponse.TimeoutInSeconds = timeout;

            bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

            Assert.IsNull(response);
            Assert.IsFalse(result);
        }

        [TestCase("user1")]
        [TestCase("user2")]
        public void TryGetResponseShouldReturnFalseIfResponseIsNotForUser(string user)
        {
            this.chatMessage.Setup(m => m.UserName).Returns(user);
            this.messageResponse.Users.Add("another user");

            bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

            Assert.IsNull(response);
            Assert.IsFalse(result);
        }

        [TestCase("keyword1", "keyword2", "message with keyword1", true)]
        [TestCase("keyword1", "keyword2", "Keyword2 message", true)]
        [TestCase("keyword1", "keyword2", "message", false)]
        public void TryGetResponseShouldReturnResponseIfMessageContainsAnyLooseKeyword(string keyword1, string keyword2, string message, bool expected)
        {
            this.chatMessage.Setup(m => m.Text).Returns(message);
            this.messageResponse.LooseKeywords.AddRange(new[] { keyword1, keyword2 });
            this.messageResponse.Replies.Add("response");

            bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expected ? "response" : null, response);
        }

        [TestCase("keyword1", "keyword2", "Keyword2 keyword1", true)]
        [TestCase("keyword1", "keyword2", "keyword2 message keyword1", true)]
        [TestCase("keyword1", "keyword2", "keyword1 message", false)]
        public void TryGetResponseShouldReturnResponseIfMessageContainsEveryAllKeyword(string keyword1, string keyword2, string message, bool expected)
        {
            this.chatMessage.Setup(m => m.Text).Returns(message);
            this.messageResponse.AllKeywords.AddRange(new[] { keyword1, keyword2 });
            this.messageResponse.Replies.Add("response");

            bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expected ? "response" : null, response);
        }

        [TestCase("keyword1", "keyword2", "keyword1", true)]
        [TestCase("keyword1", "keyword2", "keyword2", true)]
        [TestCase("keyword1", "keyword2", "keyword1 message", false)]
        public void TryGetResponseShouldReturnResponseIfMessageMatchesAnyExactKeyword(string keyword1, string keyword2, string message, bool expected)
        {
            this.chatMessage.Setup(m => m.Text).Returns(message);
            this.messageResponse.ExactKeywords.AddRange(new[] { keyword1, keyword2 });
            this.messageResponse.Replies.Add("response");

            bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expected ? "response" : null, response);
        }

        [Test]
        public void TryGetResponseShouldReturnRandomResponseWhenMultipleResponseTextsAreAvailable()
        {
            this.chatMessage.Setup(m => m.Text).Returns("keyword");
            this.messageResponse.ExactKeywords.Add("keyword");
            this.messageResponse.Replies.Add("response1");
            this.messageResponse.Replies.Add("response2");
            this.messageResponse.TimeoutInSeconds = 0;

            int response1Count = 0;
            int response2Count = 0;

            for (int i = 0; i < 100; i++)
            {
                this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

                if (response == "response1")
                {
                    response1Count++;
                }
                else
                {
                    response2Count++;
                }
            }

            Assert.AreEqual(50, response1Count, 15);
            Assert.AreEqual(50, response2Count, 15);
        }

        [Test]
        public void TryGetResponseShouldSetLastUsageDateIfMatchWasFound()
        {
            this.chatMessage.Setup(m => m.Text).Returns("keyword");
            this.messageResponse.ExactKeywords.Add("keyword");
            this.messageResponse.Replies.Add("response");

            this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

            Assert.That(this.messageResponse.LastUsed, Is.EqualTo(DateTime.Now).Within(2).Seconds);
            this.repository.Verify(r => r.AddOrUpdate(this.messageResponse));
        }

        [TestCase("response1 {{user}}", "username1", "response1 username1")]
        [TestCase("response2 {{user}}", "username2", "response2 username2")]
        public void TryGetResponseShouldSetReplaceUser(string responseText, string userName, string expected)
        {
            this.chatMessage.Setup(m => m.Text).Returns("keyword");
            this.chatMessage.Setup(m => m.UserName).Returns(userName);
            this.messageResponse.ExactKeywords.Add("keyword");
            this.messageResponse.Replies.Add(responseText);

            this.loader.TryGetResponse(this.messageResponse, this.chatMessage.Object, out string response);

            Assert.AreEqual(expected, response);
        }
    }
}