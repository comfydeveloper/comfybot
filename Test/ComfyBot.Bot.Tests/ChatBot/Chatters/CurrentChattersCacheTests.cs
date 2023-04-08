using System.Collections.Generic;
using System.Linq;
using ComfyBot.Bot.ChatBot.Chatters;
using NUnit.Framework;

namespace ComfyBot.Bot.Tests.ChatBot.Chatters;

[TestFixture]
public class CurrentChattersCacheTests
{
    private CurrentChattersCache cache;

    [SetUp]
    public void Setup()
    {
        cache = new CurrentChattersCache();
    }

    [TestCase("user1")]
    [TestCase("user2")]
    public void AddShouldAddUser(string user)
    {
        cache.Add(user);

        List<Chatter> result = cache.GetAll().ToList();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(user, result.First().Name);
        cache.Remove(user);
    }

    [Test]
    public void RemoveShouldRemoveUser()
    {
        cache.Add("user");
        cache.Remove("user");

        List<Chatter> result = cache.GetAll().ToList();

        Assert.AreEqual(0, result.Count);
    }

    [Test]
    public void RandomShouldReturnEmptyStringWhenNoElementInCache()
    {
        string result = cache.GetRandom();

        Assert.AreEqual(string.Empty, result);
    }

    [Test]
    public void RandomShouldReturnRandomElementFromCache()
    {
        cache.Add("user");

        string result = cache.GetRandom();

        Assert.AreEqual(result, "user");
        cache.Remove("user");
    }

    [Test]
    public void AddRangeShouldAddMultipleUsers()
    {
        cache.AddRange(new[] { "user1", "user2" });

        List<Chatter> result = cache.GetAll().ToList();
        Assert.AreEqual(2, result.Count);
        cache.Remove("user1");
        cache.Remove("user2");
    }
}