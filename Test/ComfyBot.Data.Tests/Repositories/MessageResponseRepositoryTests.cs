using System;
using System.Linq;
using System.Linq.Expressions;
using ComfyBot.Data.Database;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using ComfyBot.Data.Wrappers;
using LiteDB;
using Moq;
using NUnit.Framework;

namespace ComfyBot.Data.Tests.Repositories;

public class MessageResponseRepositoryTests
{
    private Mock<ILiteCollection<MessageResponseOld>> entities;

    private MessageResponseRepository repository;

    [SetUp]
    public void Setup()
    {
        Mock<IDatabaseFactory> databaseFactory = new();
        Mock<IDatabase> database = new();
        this.entities = new Mock<ILiteCollection<MessageResponseOld>>();
        database.Setup(d => d.GetCollection<MessageResponseOld>("messageResponses")).Returns(this.entities.Object);
        databaseFactory.Setup(f => f.Create()).Returns(database.Object);

        this.repository = new MessageResponseRepository(databaseFactory.Object);
    }

    [TestCase("key1")]
    [TestCase("key2")]
    public void GetShouldReturnElement(string id)
    {
        MessageResponseOld entity = new() { Id = id };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponseOld, bool>>>())).Returns(entity);

        MessageResponseOld shoutout = this.repository.Get(s => s.Id == id);

        Assert.AreEqual(entity, shoutout);
    }

    [TestCase("key1")]
    [TestCase("key2")]
    public void AddOrUpdateShouldAddNewElement(string id)
    {
        MessageResponseOld model = new();

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model));
        this.entities.Verify(e => e.Update(model), Times.Never);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void AddOrUpdateShouldUpdateTimeOut(int timeOutInSeconds)
    {
        MessageResponseOld entity = new();
        MessageResponseOld model = new() { TimeoutInSeconds = timeOutInSeconds };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponseOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model), Times.Never);
        this.entities.Verify(e => e.Update(entity));
        Assert.AreEqual(timeOutInSeconds, entity.TimeoutInSeconds);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void AddOrUpdateShouldUpdatePriority(int priority)
    {
        MessageResponseOld entity = new();
        MessageResponseOld model = new() { Priority = priority };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponseOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model), Times.Never);
        this.entities.Verify(e => e.Update(entity));
        Assert.AreEqual(priority, entity.Priority);
    }

    [TestCase(1, 2, 2)]
    [TestCase(4, 3, 4)]
    public void AddOrUpdateShouldUpdateUseCount(int newCount, int oldCount, int expected)
    {
        MessageResponseOld entity = new() { UseCount = oldCount };
        MessageResponseOld model = new() { UseCount = newCount };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponseOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        Assert.AreEqual(expected, entity.UseCount);
    }

    [TestCase("value1")]
    [TestCase("value2")]
    public void AddOrUpdateShouldUpdateCollections(string value)
    {
        MessageResponseOld entity = new()
        {
            Users = ["otherValue", value],
            AllKeywords = ["otherValue", value],
            ExactKeywords = ["otherValue", value],
            LooseKeywords = ["otherValue", value],
            Replies = ["otherValue", value]
        };
        MessageResponseOld model = new()
        {
            Users = [value],
            AllKeywords = [value],
            ExactKeywords = [value],
            LooseKeywords = [value],
            Replies = [value]
        };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponseOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model), Times.Never);
        this.entities.Verify(e => e.Update(entity));
        Assert.AreEqual(1, entity.Users.Count);
        Assert.AreEqual(1, entity.AllKeywords.Count);
        Assert.AreEqual(1, entity.ExactKeywords.Count);
        Assert.AreEqual(1, entity.LooseKeywords.Count);
        Assert.AreEqual(1, entity.Replies.Count);
        Assert.AreEqual(value, entity.Users.First());
        Assert.AreEqual(value, entity.AllKeywords.First());
        Assert.AreEqual(value, entity.ExactKeywords.First());
        Assert.AreEqual(value, entity.LooseKeywords.First());
        Assert.AreEqual(value, entity.Replies.First());
    }

    [TestCase("key1")]
    [TestCase("key2")]
    public void RemoveShouldRemoveElement(string key)
    {
        this.repository.Remove(key);

        this.entities.Verify(e => e.DeleteMany(It.IsAny<Expression<Func<MessageResponseOld, bool>>>()));
    }
}