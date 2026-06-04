using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Services.Strategies;
using FluentAssertions;
using NUnit.Framework;

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

        result.Should().Match(s => s == "test2 and test3" || s == "test1 and test3");
    }

    [Test]
    public void ReplaceShouldReplaceNumberRanges()
    {
        const string original = "[n:1-9]";

        string result = this.replacer.Replace(original);

        int resultNumber = int.Parse(result);
        resultNumber.Should().BeInRange(1, 9);
    }
}
