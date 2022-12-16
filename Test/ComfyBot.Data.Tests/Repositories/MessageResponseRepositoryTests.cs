using LiteDB;

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

    public class MessageResponseRepositoryTests
    {
        private Mock<ILiteCollection<MessageResponse>> entities;

        private MessageResponseRepository repository;

        [SetUp]
        public void Setup()
        {
            Mock<IDatabaseFactory> databaseFactory = new Mock<IDatabaseFactory>();
            Mock<IDatabase> database = new Mock<IDatabase>();
            this.entities = new Mock<ILiteCollection<MessageResponse>>();
            database.Setup(d => d.GetCollection<MessageResponse>("messageResponses")).Returns(this.entities.Object);
            databaseFactory.Setup(f => f.Create()).Returns(database.Object);

            this.repository = new MessageResponseRepository(databaseFactory.Object);
        }

        [TestCase("key1")]
        [TestCase("key2")]
        public void GetShouldReturnElement(string id)
        {
            MessageResponse entity = new MessageResponse { Id = id };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponse, bool>>>())).Returns(entity);

            MessageResponse shoutout = this.repository.Get(s => s.Id == id);

            Assert.AreEqual(entity, shoutout);
        }

        [TestCase("key1")]
        [TestCase("key2")]
        public void AddOrUpdateShouldAddNewElement(string id)
        {
            MessageResponse model = new MessageResponse();

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model));
            this.entities.Verify(e => e.Update(model), Times.Never);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void AddOrUpdateShouldUpdateTimeOut(int timeOutInSeconds)
        {
            MessageResponse entity = new MessageResponse();
            MessageResponse model = new MessageResponse { TimeoutInSeconds = timeOutInSeconds };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponse, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model), Times.Never);
            this.entities.Verify(e => e.Update(entity));
            Assert.AreEqual(timeOutInSeconds, entity.TimeoutInSeconds);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void AddOrUpdateShouldUpdatePriority(int priority)
        {
            MessageResponse entity = new MessageResponse();
            MessageResponse model = new MessageResponse { Priority = priority };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponse, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model), Times.Never);
            this.entities.Verify(e => e.Update(entity));
            Assert.AreEqual(priority, entity.Priority);
        }

        [TestCase(1, 2, 2)]
        [TestCase(4, 3, 4)]
        public void AddOrUpdateShouldUpdateUseCount(int newCount, int oldCount, int expected)
        {
            MessageResponse entity = new MessageResponse { UseCount = oldCount };
            MessageResponse model = new MessageResponse { UseCount = newCount };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponse, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

            Assert.AreEqual(expected, entity.UseCount);
        }

        [TestCase("value1")]
        [TestCase("value2")]
        public void AddOrUpdateShouldUpdateCollections(string value)
        {
            MessageResponse entity = new MessageResponse
                                     {
                                         Users = new List<string> { "otherValue", value},
                                         AllKeywords = new List<string> { "otherValue", value},
                                         ExactKeywords = new List<string> { "otherValue", value},
                                         LooseKeywords = new List<string> { "otherValue", value},
                                         Replies = new List<string> { "otherValue", value}
                                     };
            MessageResponse model = new MessageResponse {
                                                            Users = new List<string> { value },
                                                            AllKeywords = new List<string> { value },
                                                            ExactKeywords = new List<string> { value },
                                                            LooseKeywords = new List<string> { value },
                                                            Replies = new List<string> { value }
                                                        };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<MessageResponse, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

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

            this.entities.Verify(e => e.DeleteMany(It.IsAny<Expression<Func<MessageResponse, bool>>>()));
        }
    }
}