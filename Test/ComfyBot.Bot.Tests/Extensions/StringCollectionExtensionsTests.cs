using System.Collections.Generic;
using ComfyBot.Bot.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.Extensions;

[TestFixture]
public class StringCollectionExtensionsTests
{
    [Test]
    public void GetRandomShouldReturnRandomElementFromCollection()
    {
        List<string> collection = ["random1", "random2"];
        int elementCount1 = 0;
        int elementCount2 = 0;

        for (int i = 0; i < 100; i++)
        {
            string random = collection.GetRandom();

            if (random == "random1")
            {
                elementCount1++;
            }
            else
            {
                elementCount2++;
            }
        }

        elementCount1.Should().BeInRange(35, 65);
        elementCount2.Should().BeInRange(35, 65);
    }

    [TestCase("string1")]
    [TestCase("string2")]
    public void GetRandomShouldReturnOnlyElement(string text)
    {
        List<string> collection = [text];

        string result = collection.GetRandom();

        result.Should().Be(text);
    }
}
