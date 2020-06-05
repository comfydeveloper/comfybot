namespace ComfyBot.Bot.Tests.Extensions
{
    using ComfyBot.Bot.Extensions;

    using NUnit.Framework;

    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("{{parameter1}}", 1, true)]
        [TestCase("{{parameter2}}", 1, false)]
        [TestCase("{{parameter1}}", 2, true)]
        [TestCase("{{parameter10}} {{parameter1}}", 2, false)]
        public void CanHandleParametersShouldReturnCorrectResult(string reply, int parameterCount, bool expected)
        {
            bool result = reply.CanHandleParameters(parameterCount);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanHandleShouldOnlyParseNumbers()
        {
            bool result = "{{parameters}}".CanHandleParameters(1);

            Assert.True(result);
        }

        [TestCase("test-test", "test", "-test")]
        [TestCase("test-test", "test2", "test-test")]
        public void ReplaceFirstShouldReplaceFirstOccurrenceOfSubstring(string original, string keyword, string expected)
        {
            string result = original.ReplaceFirst(keyword, "");

            Assert.AreEqual(expected, result);
        }
    }
}