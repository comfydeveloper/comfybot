namespace ComfyBot.Data.Tests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using ComfyBot.Data.Database;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;
    using ComfyBot.Data.Wrappers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class TextCommandRepositoryTests
    {
        private Mock<ILiteCollection<TextCommand>> entities;

        private TextCommandRepository repository;

        [SetUp]
        public void Setup()
        {
            Mock<IDatabaseFactory> databaseFactory = new Mock<IDatabaseFactory>();
            Mock<IDatabase> database = new Mock<IDatabase>();
            this.entities = new Mock<ILiteCollection<TextCommand>>();
            database.Setup(d => d.GetCollection<TextCommand>("textCommands")).Returns(this.entities.Object);
            databaseFactory.Setup(f => f.Create()).Returns(database.Object);

            this.repository = new TextCommandRepository(databaseFactory.Object);
        }

        [TestCase("key1")]
        [TestCase("key2")]
        public void GetShouldReturnElement(string id)
        {
            TextCommand entity = new TextCommand { Id = id };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommand, bool>>>())).Returns(entity);

            TextCommand shoutout = this.repository.Get(s => s.Id == id);

            Assert.AreEqual(entity, shoutout);
        }

        [TestCase("key1")]
        [TestCase("key2")]
        public void AddOrUpdateShouldAddNewElement(string id)
        {
            TextCommand model = new TextCommand();

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model));
            this.entities.Verify(e => e.Update(model), Times.Never);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void AddOrUpdateShouldUpdateTimeOut(int timeOutInSeconds)
        {
            TextCommand entity = new TextCommand();
            TextCommand model = new TextCommand { TimeoutInSeconds = timeOutInSeconds };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommand, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model), Times.Never);
            this.entities.Verify(e => e.Update(entity));
            Assert.AreEqual(timeOutInSeconds, entity.TimeoutInSeconds);
        }

        [TestCase("command1")]
        [TestCase("command2")]
        public void AddOrUpdateShouldUpdateCommand(string command)
        {
            TextCommand entity = new TextCommand();
            TextCommand model = new TextCommand { Command = command };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommand, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model), Times.Never);
            this.entities.Verify(e => e.Update(entity));
            Assert.AreEqual(command, entity.Command);
        }

        [TestCase("2020-01-01")]
        [TestCase("2020-01-02")]
        public void AddOrUpdateShouldUpdateUsedTime(DateTime lastUsedTime)
        {
            TextCommand entity = new TextCommand();
            TextCommand model = new TextCommand { LastUsed = lastUsedTime };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommand, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model), Times.Never);
            this.entities.Verify(e => e.Update(entity));
            Assert.AreEqual(lastUsedTime, entity.LastUsed);
        }

        [TestCase("value1")]
        [TestCase("value2")]
        public void AddOrUpdateShouldUpdateCollection(string value)
        {
            TextCommand entity = new TextCommand
            {
                Replies = new List<string> { "otherValue", value }
            };
            TextCommand model = new TextCommand
            {
                Replies = new List<string> { value }
            };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<TextCommand, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

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

            this.entities.Verify(e => e.Remove(It.IsAny<Expression<Func<TextCommand, bool>>>()));
        }
    }
}