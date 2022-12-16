namespace ComfyBot.Bot.Tests.Extensions;

using System.Collections.Generic;

using ComfyBot.Bot.Extensions;

using NUnit.Framework;

[TestFixture]
public class StringCollectionExtensionsTests
{
    [Test]
    public void GetRandomShouldReturnRandomElementFromCollection()
    {
        List<string> collection = new List<string> { "random1", "random2" };
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

        Assert.AreEqual(50, elementCount1, 15);
        Assert.AreEqual(50, elementCount2, 15);
    }

    [TestCase("string1")]
    [TestCase("string2")]
    public void GetRandomShouldReturnOnlyElement(string text)
    {
        List<string> collection = new List<string> { text };

        string result = collection.GetRandom();

        Assert.AreEqual(text, result);
    }
}