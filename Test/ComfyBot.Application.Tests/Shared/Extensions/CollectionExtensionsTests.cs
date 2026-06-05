using System.Collections.Generic;
using System.Linq;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Extensions;
using NUnit.Framework;
using Shouldly;
using System.Collections.ObjectModel;

namespace ComfyBot.Application.Tests.Shared.Extensions;

[TestFixture]
public class CollectionExtensionsTests
{
    [TestCase("text1")]
    [TestCase("text2")]
    public void ToTextModelsShouldMapStringsToTextModels(string text)
    {
        string[] source = [text, ""];

        IEnumerable<TextModel> result = source.ToTextModels().ToArray();

        result.Count().ShouldBe(2);
        result.First().Text.ShouldBe(text);
    }
}