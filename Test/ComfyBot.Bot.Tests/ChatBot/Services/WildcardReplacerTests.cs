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
        const string Original = "[w:test2,test1] and [w:test3]";

        string result = replacer.Replace(Original);

        Assert.That(result == "test2 and test3" || result == "test1 and test3");
    }

    [Test]
    public void ReplaceShouldReplaceNumberRanges()
    {
        const string Original = "[n:1-9]";

        string result = replacer.Replace(Original);

        int resultNumber = int.Parse(result);
        Assert.That(resultNumber > 0 && resultNumber < 10);
    }

    [Test]
    public void ReplaceShouldReplaceRandomChatter()
    {
        chattersCache.Setup(c => c.GetRandom()).Returns("user");
        const string Original = "{{chatter}}";

        string result = replacer.Replace(Original);

        Assert.AreEqual("user", result);
    }
}