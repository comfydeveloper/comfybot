using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Services.Strategies;
using NUnit.Framework;
using Shouldly;

namespace ComfyBot.Bot.Tests.ChatBot.Services;

public class WildcardReplacerTests
{
    private WildcardReplacer replacer;

    [SetUp]
    public void Setup()
    {
        this.replacer = new WildcardReplacer([new VariableWordsStrategy(), new NumberRangeStrategy()]);
    }

    [Test]
    public void ReplaceShouldReplaceVariableTextParts()
    {
        const string original = "[w:test2,test1] and [w:test3]";

        string result = this.replacer.Replace(original);

        result.ShouldBeOneOf("test2 and test3", "test1 and test3");
    }

    [Test]
    public void ReplaceShouldReplaceNumberRanges()
    {
        const string original = "[n:1-9]";

        string result = this.replacer.Replace(original);

        int resultNumber = int.Parse(result);
        resultNumber.ShouldBeInRange(1, 9);
    }
}
