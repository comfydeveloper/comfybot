namespace ComfyBot.Bot.Tests.ChatBot.Chatters
{
    using System.Collections.Generic;
    using System.Linq;

    using ComfyBot.Bot.ChatBot.Chatters;

    using NUnit.Framework;

    [TestFixture]
    public class CurrentChattersCacheTests
    {
        private CurrentChattersCache cache;

        [SetUp]
        public void Setup()
        {
            this.cache = new CurrentChattersCache();
        }

        [TestCase("user1")]
        [TestCase("user2")]
        public void AddShouldAddUser(string user)
        {
            this.cache.Add(user);

            List<string> result = this.cache.GetAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(user, result.First());
            this.cache.Remove(user);
        }

        [Test]
        public void RemoveShouldRemoveUser()
        {
            this.cache.Add("user");
            this.cache.Remove("user");

            List<string> result = this.cache.GetAll().ToList();

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void RandomShouldReturnEmptyStringWhenNoElementInCache()
        {
            string result = this.cache.GetRandom();

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void RandomShouldReturnRandomElementFromCache()
        {
            this.cache.Add("user");

            string result = this.cache.GetRandom();

            Assert.AreEqual(result, "user");
            this.cache.Remove("user");
        }

        [Test]
        public void AddRangeShouldAddMultipleUsers()
        {
            this.cache.AddRange(new[] { "user1", "user2" });

            List<string> result = this.cache.GetAll().ToList();
            Assert.AreEqual(2, result.Count);
            this.cache.Remove("user1");
            this.cache.Remove("user2");

        }
    }
}