namespace ComfyBot.Bot.Tests.ChatBot.Services;

using ComfyBot.Bot.ChatBot.Chatters;
using ComfyBot.Bot.ChatBot.Services;

using Moq;

using NUnit.Framework;

public class WildcardReplacerTests
{
    private Mock<IChattersCache> chattersCache;

    private WildcardReplacer replacer;

    [SetUp]
    public void Setup()
    {
        chattersCache = new Mock<IChattersCache>();

        replacer = new WildcardReplacer(chattersCache.Object);
    }

    [Test]
    public void ReplaceShouldReplaceVariableTextParts()
    {
        const string original = "[w:test2,test1] and [w:test3]";

        string result = replacer.Replace(original);

        Assert.That(result is "test2 and test3" or "test1 and test3");
    }

    [Test]
    public void ReplaceShouldReplaceNumberRanges()
    {
        const string original = "[n:1-9]";

        string result = replacer.Replace(original);

        int resultNumber = int.Parse(result);
        Assert.That(resultNumber is > 0 and < 10);
    }

    [Test]
    public void ReplaceShouldReplaceRandomChatter()
    {
        chattersCache.Setup(c => c.GetRandom()).Returns("user");
        const string original = "{{chatter}}";

        string result = replacer.Replace(original);

        Assert.AreEqual("user", result);
    }
}