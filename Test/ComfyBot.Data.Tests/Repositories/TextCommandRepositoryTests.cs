using System;
using System.Linq;
using System.Linq.Expressions;
using ComfyBot.Data.Database;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using ComfyBot.Data.Wrappers;
using LiteDB;
using NUnit.Framework;

namespace ComfyBot.Data.Tests.Repositories;

[TestFixture]
public class TextCommandRepositoryTests
{
    private Mock<ILiteCollection<TextCommandOld>> entities;

    private TextCommandRepository repository;

    [SetUp]
    public void Setup()
    {
        Mock<IDatabaseFactory> databaseFactory = new();
        Mock<IDatabase> database = new();
        this.entities = new Mock<ILiteCollection<TextCommandOld>>();
        database.Setup(d => d.GetCollection<TextCommandOld>("textCommands")).Returns(this.entities.Object);
        databaseFactory.Setup(f => f.Create()).Returns(database.Object);

        this.repository = new TextCommandRepository(databaseFactory.Object);
    }

    [TestCase("key1")]
    [TestCase("key2")]
    public void GetShouldReturnElement(string id)
    {
        TextCommandOld entity = new() { Id = id };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommandOld, bool>>>())).Returns(entity);

        TextCommandOld shoutout = this.repository.Get(s => s.Id == id);

        Assert.AreEqual(entity, shoutout);
    }

    [TestCase("key1")]
    [TestCase("key2")]
    public void AddOrUpdateShouldAddNewElement(string id)
    {
        TextCommandOld model = new();

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model));
        this.entities.Verify(e => e.Update(model), Times.Never);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void AddOrUpdateShouldUpdateTimeOut(int timeOutInSeconds)
    {
        TextCommandOld entity = new();
        TextCommandOld model = new() { TimeoutInSeconds = timeOutInSeconds };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommandOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model), Times.Never);
        this.entities.Verify(e => e.Update(entity));
        Assert.AreEqual(timeOutInSeconds, entity.TimeoutInSeconds);
    }

    [TestCase("value1")]
    [TestCase("value2")]
    public void AddOrUpdateShouldUpdateCommandsCollection(string value)
    {
        TextCommandOld entity = new()
        {
            Commands = ["otherValue", value]
        };
        TextCommandOld model = new()
        {
            Commands = [value]
        };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommandOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model), Times.Never);
        this.entities.Verify(e => e.Update(entity));
        Assert.AreEqual(1, entity.Commands.Count);
        Assert.AreEqual(value, entity.Commands.First());
    }

    [TestCase("2020-01-01")]
    [TestCase("2020-01-02")]
    public void AddOrUpdateShouldUpdateUsedTime(DateTime lastUsedTime)
    {
        TextCommandOld entity = new();
        TextCommandOld model = new() { LastUsed = lastUsedTime };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommandOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model), Times.Never);
        this.entities.Verify(e => e.Update(entity));
        Assert.AreEqual(lastUsedTime, entity.LastUsed);
    }

    [TestCase(1)]
    [TestCase(2)]
    public void AddOrUpdateShouldUpdateUseCount(int count)
    {
        TextCommandOld entity = new();
        TextCommandOld model = new() { UseCount = count };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommandOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        Assert.AreEqual(count, entity.UseCount);
    }

    [TestCase("value1")]
    [TestCase("value2")]
    public void AddOrUpdateShouldUpdateCollection(string value)
    {
        TextCommandOld entity = new()
        {
            Replies = ["otherValue", value]
        };
        TextCommandOld model = new()
        {
            Replies = [value]
        };
        this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommandOld, bool>>>())).Returns(entity);

        this.repository.Write(model);

        this.entities.Verify(e => e.Insert(model), Times.Never);
        this.entities.Verify(e => e.Update(entity));
        Assert.AreEqual(1, entity.Replies.Count);
        Assert.AreEqual(value, entity.Replies.First());
    }

    [TestCase("key1")]
    [TestCase("key2")]
    public void RemoveShouldRemoveElement(string key)
    {
        this.repository.Remove(key);

        this.entities.Verify(e => e.DeleteMany(It.IsAny<Expression<Func<TextCommandOld, bool>>>()));
    }
}