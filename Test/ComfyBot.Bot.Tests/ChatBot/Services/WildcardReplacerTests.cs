namespace ComfyBot.Bot.Tests.ChatBot.Services
{
    using ComfyBot.Bot.ChatBot.Services;

    using NUnit.Framework;

    public class WildcardReplacerTests
    {
        private WildcardReplacer replacer;

        [SetUp]
        public void Setup()
        {
            this.replacer = new WildcardReplacer();
        }

        [Test]
        public void ReplaceShouldReplaceVariableTextParts()
        {
            const string Original = "[w:test2,test1] and [w:test3]";

            string result = this.replacer.Replace(Original);

            Assert.That(result == "test2 and test3" || result == "test1 and test3");
        }

        [Test]
        public void ReplaceShouldReplaceNumberRanges()
        {
            const string Original = "[n:1-9]";

            string result = this.replacer.Replace(Original);

            int resultNumber = int.Parse(result);
            Assert.That(resultNumber > 0 && resultNumber < 10);
        }
    }
}