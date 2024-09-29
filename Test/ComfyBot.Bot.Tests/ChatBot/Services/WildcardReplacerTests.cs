using ComfyBot.Bot.ChatBot.Services;
using Moq;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Services;

public class WildcardReplacerTests
{
    private WildcardReplacer replacer;

    [SetUp]
    public void Setup()
    {

        replacer = new WildcardReplacer();
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
}