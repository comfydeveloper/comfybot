using System;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Moq;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Messages;

[TestFixture]
public class MessageResponseLoaderTests
{
    private Mock<IRepository<MessageResponseOld>> repository;
    private Mock<IWildcardReplacer> wildcardReplacer;
    private Mock<IChatMessage> chatMessage;

    private MessageResponseOld messageResponseOld;

    private MessageResponseLoader loader;

    [SetUp]
    public void Setup()
    {
        this.repository = new Mock<IRepository<MessageResponseOld>>();
        this.wildcardReplacer = new Mock<IWildcardReplacer>();
        this.chatMessage = new Mock<IChatMessage>();
        this.wildcardReplacer.Setup(r => r.Replace(It.IsAny<string>())).Returns<string>(s => s);

        this.messageResponseOld = new MessageResponseOld();

        this.loader = new MessageResponseLoader(this.repository.Object, this.wildcardReplacer.Object);
    }

    [TestCase(10)]
    [TestCase(20)]
    public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
    {
        this.messageResponseOld.LastUsed = DateTime.Now.AddSeconds(-timeout + 1);
        this.messageResponseOld.TimeoutInSeconds = timeout;

        bool result = this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.IsNull(response);
        Assert.IsFalse(result);
    }

    [TestCase("user1")]
    [TestCase("user2")]
    public void TryGetResponseShouldReturnFalseIfResponseIsNotForUser(string user)
    {
        this.chatMessage.Setup(m => m.UserName).Returns(user);
        this.messageResponseOld.Users.Add("another user");

        bool result = this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.IsNull(response);
        Assert.IsFalse(result);
    }

    [TestCase("keyword1", "keyword2", "message with keyword1", true)]
    [TestCase("keyword1", "keyword2", "Keyword2 message", true)]
    [TestCase("keyword1", "keyword2", "message", false)]
    public void TryGetResponseShouldReturnResponseIfMessageContainsAnyLooseKeyword(string keyword1, string keyword2, string message, bool expected)
    {
        this.chatMessage.Setup(m => m.Text).Returns(message);
        this.messageResponseOld.LooseKeywords.AddRange(new[] { keyword1, keyword2 });
        this.messageResponseOld.Replies.Add("responseOld");

        bool result = this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.AreEqual(expected, result);
        Assert.AreEqual(expected ? "responseOld" : null, response);
    }

    [TestCase("keyword1", "keyword2", "Keyword2 keyword1", true)]
    [TestCase("keyword1", "keyword2", "keyword2 message keyword1", true)]
    [TestCase("keyword1", "keyword2", "keyword1 message", false)]
    public void TryGetResponseShouldReturnResponseIfMessageContainsEveryAllKeyword(string keyword1, string keyword2, string message, bool expected)
    {
        this.chatMessage.Setup(m => m.Text).Returns(message);
        this.messageResponseOld.AllKeywords.AddRange(new[] { keyword1, keyword2 });
        this.messageResponseOld.Replies.Add("responseOld");

        bool result = this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.AreEqual(expected, result);
        Assert.AreEqual(expected ? "responseOld" : null, response);
    }

    [Test]
    public void ShouldReturnMessageWhenSetToAlwaysReply()
    {
        this.messageResponseOld.ReplyAlways = true;
        this.messageResponseOld.Replies.Add("responseOld");

        bool result = this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.True(result);
        Assert.NotNull(response);
    }

    [TestCase("keyword1", "keyword2", "keyword1", true)]
    [TestCase("keyword1", "keyword2", "keyword2", true)]
    [TestCase("keyword1", "keyword2", "keyword1 message", false)]
    public void TryGetResponseShouldReturnResponseIfMessageMatchesAnyExactKeyword(string keyword1, string keyword2, string message, bool expected)
    {
        this.chatMessage.Setup(m => m.Text).Returns(message);
        this.messageResponseOld.ExactKeywords.AddRange(new[] { keyword1, keyword2 });
        this.messageResponseOld.Replies.Add("responseOld");

        bool result = this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.AreEqual(expected, result);
        Assert.AreEqual(expected ? "responseOld" : null, response);
    }

    [Test]
    public void TryGetResponseShouldReturnRandomResponseWhenMultipleResponseTextsAreAvailable()
    {
        this.chatMessage.Setup(m => m.Text).Returns("keyword");
        this.messageResponseOld.ExactKeywords.Add("keyword");
        this.messageResponseOld.Replies.Add("response1");
        this.messageResponseOld.Replies.Add("response2");
        this.messageResponseOld.TimeoutInSeconds = 0;

        int response1Count = 0;
        int response2Count = 0;

        for (int i = 0; i < 100; i++)
        {
            this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

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
        this.messageResponseOld.ExactKeywords.Add("keyword");
        this.messageResponseOld.Replies.Add("responseOld");

        this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.That(this.messageResponseOld.LastUsed, Is.EqualTo(DateTime.Now).Within(2).Seconds);
        this.repository.Verify(r => r.Write(this.messageResponseOld));
    }

    [Test]
    public void TryGetResponseShouldSetUseCountIfMatchWasFound()
    {
        this.chatMessage.Setup(m => m.Text).Returns("keyword");
        this.messageResponseOld.ExactKeywords.Add("keyword");
        this.messageResponseOld.Replies.Add("responseOld");

        this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.AreEqual(1, this.messageResponseOld.UseCount);
    }

    [TestCase("response1 {{user}}", "username1", "response1 username1")]
    [TestCase("response2 {{user}}", "username2", "response2 username2")]
    public void TryGetResponseShouldSetReplaceUser(string responseText, string userName, string expected)
    {
        this.chatMessage.Setup(m => m.Text).Returns("keyword");
        this.chatMessage.Setup(m => m.UserName).Returns(userName);
        this.messageResponseOld.ExactKeywords.Add("keyword");
        this.messageResponseOld.Replies.Add(responseText);

        this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        Assert.AreEqual(expected, response);
    }

    [Test]
    public void TryGetResponseShouldCallReplacementService()
    {
        string responseText = "responseOld";
        this.chatMessage.Setup(m => m.Text).Returns("keyword");
        this.messageResponseOld.ExactKeywords.Add("keyword");
        this.messageResponseOld.Replies.Add(responseText);

        this.loader.TryGetResponse(this.messageResponseOld, this.chatMessage.Object, out string response);

        this.wildcardReplacer.Verify(r => r.Replace(responseText));
    }
}