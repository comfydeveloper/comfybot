namespace ComfyBot.Bot.Tests.ChatBot.Messages
{
    using System;

    using ComfyBot.Bot.ChatBot.Messages;
    using ComfyBot.Bot.ChatBot.Services;
    using ComfyBot.Bot.ChatBot.Wrappers;
    using Data.Models;
    using Data.Repositories;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class MessageResponseLoaderTests
    {
        private Mock<IRepository<MessageResponse>> repository;
        private Mock<IWildcardReplacer> wildcardReplacer;
        private Mock<IChatMessage> chatMessage;

        private MessageResponse messageResponse;

        private MessageResponseLoader loader;

        [SetUp]
        public void Setup()
        {
            repository = new Mock<IRepository<MessageResponse>>();
            wildcardReplacer = new Mock<IWildcardReplacer>();
            chatMessage = new Mock<IChatMessage>();
            wildcardReplacer.Setup(r => r.Replace(It.IsAny<string>())).Returns<string>(s => s);

            messageResponse = new MessageResponse();

            loader = new MessageResponseLoader(repository.Object, wildcardReplacer.Object);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
        {
            messageResponse.LastUsed = DateTime.Now.AddSeconds(-timeout + 1);
            messageResponse.TimeoutInSeconds = timeout;

            bool result = loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.IsNull(response);
            Assert.IsFalse(result);
        }

        [TestCase("user1")]
        [TestCase("user2")]
        public void TryGetResponseShouldReturnFalseIfResponseIsNotForUser(string user)
        {
            chatMessage.Setup(m => m.UserName).Returns(user);
            messageResponse.Users.Add("another user");

            bool result = loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.IsNull(response);
            Assert.IsFalse(result);
        }

        [TestCase("keyword1", "keyword2", "message with keyword1", true)]
        [TestCase("keyword1", "keyword2", "Keyword2 message", true)]
        [TestCase("keyword1", "keyword2", "message", false)]
        public void TryGetResponseShouldReturnResponseIfMessageContainsAnyLooseKeyword(string keyword1, string keyword2, string message, bool expected)
        {
            chatMessage.Setup(m => m.Text).Returns(message);
            messageResponse.LooseKeywords.AddRange(new[] { keyword1, keyword2 });
            messageResponse.Replies.Add("response");

            bool result = loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expected ? "response" : null, response);
        }

        [TestCase("keyword1", "keyword2", "Keyword2 keyword1", true)]
        [TestCase("keyword1", "keyword2", "keyword2 message keyword1", true)]
        [TestCase("keyword1", "keyword2", "keyword1 message", false)]
        public void TryGetResponseShouldReturnResponseIfMessageContainsEveryAllKeyword(string keyword1, string keyword2, string message, bool expected)
        {
            chatMessage.Setup(m => m.Text).Returns(message);
            messageResponse.AllKeywords.AddRange(new[] { keyword1, keyword2 });
            messageResponse.Replies.Add("response");

            bool result = loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expected ? "response" : null, response);
        }

        [Test]
        public void ShouldReturnMessageWhenSetToAlwaysReply()
        {
            messageResponse.ReplyAlways = true;

            bool result = loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.True(result);
            Assert.NotNull(response);
        }

        [TestCase("keyword1", "keyword2", "keyword1", true)]
        [TestCase("keyword1", "keyword2", "keyword2", true)]
        [TestCase("keyword1", "keyword2", "keyword1 message", false)]
        public void TryGetResponseShouldReturnResponseIfMessageMatchesAnyExactKeyword(string keyword1, string keyword2, string message, bool expected)
        {
            chatMessage.Setup(m => m.Text).Returns(message);
            messageResponse.ExactKeywords.AddRange(new[] { keyword1, keyword2 });
            messageResponse.Replies.Add("response");

            bool result = loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expected ? "response" : null, response);
        }

        [Test]
        public void TryGetResponseShouldReturnRandomResponseWhenMultipleResponseTextsAreAvailable()
        {
            chatMessage.Setup(m => m.Text).Returns("keyword");
            messageResponse.ExactKeywords.Add("keyword");
            messageResponse.Replies.Add("response1");
            messageResponse.Replies.Add("response2");
            messageResponse.TimeoutInSeconds = 0;

            int response1Count = 0;
            int response2Count = 0;

            for (int i = 0; i < 100; i++)
            {
                loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

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
            chatMessage.Setup(m => m.Text).Returns("keyword");
            messageResponse.ExactKeywords.Add("keyword");
            messageResponse.Replies.Add("response");

            loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.That(messageResponse.LastUsed, Is.EqualTo(DateTime.Now).Within(2).Seconds);
            repository.Verify(r => r.AddOrUpdate(messageResponse));
        }

        [Test]
        public void TryGetResponseShouldSetUseCountIfMatchWasFound()
        {
            chatMessage.Setup(m => m.Text).Returns("keyword");
            messageResponse.ExactKeywords.Add("keyword");
            messageResponse.Replies.Add("response");

            loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.AreEqual(1, messageResponse.UseCount);
        }

        [TestCase("response1 {{user}}", "username1", "response1 username1")]
        [TestCase("response2 {{user}}", "username2", "response2 username2")]
        public void TryGetResponseShouldSetReplaceUser(string responseText, string userName, string expected)
        {
            chatMessage.Setup(m => m.Text).Returns("keyword");
            chatMessage.Setup(m => m.UserName).Returns(userName);
            messageResponse.ExactKeywords.Add("keyword");
            messageResponse.Replies.Add(responseText);

            loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            Assert.AreEqual(expected, response);
        }

        [Test]
        public void TryGetResponseShouldCallReplacementService()
        {
            string responseText = "response";
            chatMessage.Setup(m => m.Text).Returns("keyword");
            messageResponse.ExactKeywords.Add("keyword");
            messageResponse.Replies.Add(responseText);

            loader.TryGetResponse(messageResponse, chatMessage.Object, out string response);

            wildcardReplacer.Verify(r => r.Replace(responseText));
        }
    }
}