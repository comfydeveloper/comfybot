using System;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Data.Models;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace ComfyBot.Bot.Tests.ChatBot.Messages;

[TestFixture]
public class MessageResponseLoaderTests
{
    private IWildcardReplacer wildcardReplacer;
    private IChatMessage chatMessage;

    private MessageResponse messageResponse;

    private MessageResponseLoader loader;

    [SetUp]
    public void Setup()
    {
        this.wildcardReplacer = Substitute.For<IWildcardReplacer>();
        this.chatMessage = Substitute.For<IChatMessage>();
        this.wildcardReplacer.Replace(Arg.Any<string>()).Returns(s => (string)s[0]);

        this.messageResponse = CreateMessageResponse();

        this.loader = new MessageResponseLoader(this.wildcardReplacer);
    }

    private static MessageResponse CreateMessageResponse()
    {
        return new MessageResponse
        {
            Users = [],
            LooseKeywords = [],
            AllKeywords = [],
            ExactKeywords = [],
            Replies = [],
            LastUsedAt = DateTime.UtcNow,
            TimeoutInSeconds = 0,
            UseCount = 0,
            Priority = 0,
            AlwaysReply = false,
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
    }

    [TestCase(10)]
    [TestCase(20)]
    public void TryGetResponseShouldReturnFalseWhenTheResponseTimeoutHasNotRunOutYet(int timeout)
    {
        this.messageResponse.LastUsedAt = DateTime.UtcNow.AddSeconds(-timeout + 1);
        this.messageResponse.TimeoutInSeconds = timeout;

        bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        response.ShouldBeNull();
        result.ShouldBeFalse();
    }

    [TestCase("user1")]
    [TestCase("user2")]
    public void TryGetResponseShouldReturnFalseIfResponseIsNotForUser(string user)
    {
        this.chatMessage.UserName.Returns(user);
        this.messageResponse.Users.Add("another user");

        bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        response.ShouldBeNull();
        result.ShouldBeFalse();
    }

    [TestCase("keyword1", "keyword2", "message with keyword1", true)]
    [TestCase("keyword1", "keyword2", "Keyword2 message", true)]
    [TestCase("keyword1", "keyword2", "message", false)]
    public void TryGetResponseShouldReturnResponseIfMessageContainsAnyLooseKeyword(string keyword1, string keyword2, string message, bool expected)
    {
        this.chatMessage.Text.Returns(message);
        this.messageResponse.LooseKeywords.AddRange(new[] { keyword1, keyword2 });
        this.messageResponse.Replies.Add("response");

        bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        result.ShouldBe(expected);
        response.ShouldBe(expected ? "response" : null);
    }

    [TestCase("keyword1", "keyword2", "Keyword2 keyword1", true)]
    [TestCase("keyword1", "keyword2", "keyword2 message keyword1", true)]
    [TestCase("keyword1", "keyword2", "keyword1 message", false)]
    public void TryGetResponseShouldReturnResponseIfMessageContainsEveryAllKeyword(string keyword1, string keyword2, string message, bool expected)
    {
        this.chatMessage.Text.Returns(message);
        this.messageResponse.AllKeywords.AddRange(new[] { keyword1, keyword2 });
        this.messageResponse.Replies.Add("response");

        bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        result.ShouldBe(expected);
        response.ShouldBe(expected ? "response" : null);
    }

    [Test]
    public void ShouldReturnMessageWhenSetToAlwaysReply()
    {
        this.messageResponse.AlwaysReply = true;
        this.messageResponse.Replies.Add("response");

        bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        result.ShouldBeTrue();
        response.ShouldNotBeNull();
    }

    [TestCase("keyword1", "keyword2", "keyword1", true)]
    [TestCase("keyword1", "keyword2", "keyword2", true)]
    [TestCase("keyword1", "keyword2", "keyword1 message", false)]
    public void TryGetResponseShouldReturnResponseIfMessageMatchesAnyExactKeyword(string keyword1, string keyword2, string message, bool expected)
    {
        this.chatMessage.Text.Returns(message);
        this.messageResponse.ExactKeywords.AddRange(new[] { keyword1, keyword2 });
        this.messageResponse.Replies.Add("response");

        bool result = this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        result.ShouldBe(expected);
        response.ShouldBe(expected ? "response" : null);
    }

    [Test]
    public void TryGetResponseShouldReturnRandomResponseWhenMultipleResponseTextsAreAvailable()
    {
        this.chatMessage.Text.Returns("keyword");
        this.messageResponse.ExactKeywords.Add("keyword");
        this.messageResponse.Replies.Add("response1");
        this.messageResponse.Replies.Add("response2");
        this.messageResponse.TimeoutInSeconds = 0;

        int response1Count = 0;
        int response2Count = 0;

        for (int i = 0; i < 100; i++)
        {
            this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

            if (response == "response1")
            {
                response1Count++;
            }
            else
            {
                response2Count++;
            }
        }

        response1Count.ShouldBeInRange(35, 65);
    }

    [TestCase("response1 {{user}}", "username1", "response1 username1")]
    [TestCase("response2 {{user}}", "username2", "response2 username2")]
    public void TryGetResponseShouldSetReplaceUser(string responseText, string userName, string expected)
    {
        this.chatMessage.Text.Returns("keyword");
        this.chatMessage.UserName.Returns(userName);
        this.messageResponse.ExactKeywords.Add("keyword");
        this.messageResponse.Replies.Add(responseText);

        this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        response.ShouldBe(expected);
    }

    [Test]
    public void TryGetResponseShouldCallReplacementService()
    {
        string responseText = "response";
        this.chatMessage.Text.Returns("keyword");
        this.messageResponse.ExactKeywords.Add("keyword");
        this.messageResponse.Replies.Add(responseText);

        this.loader.TryGetResponse(this.messageResponse, this.chatMessage, out string response);

        this.wildcardReplacer.Received(1).Replace(responseText);
    }
}
