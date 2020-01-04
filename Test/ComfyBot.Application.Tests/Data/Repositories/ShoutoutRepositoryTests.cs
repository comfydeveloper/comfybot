namespace ComfyBot.Application.Tests.Data.Repositories
{
    using System;
    using System.Linq.Expressions;

    using ComfyBot.Application.Data;
    using ComfyBot.Application.Data.Models;
    using ComfyBot.Application.Data.Repositories;
    using ComfyBot.Application.Data.Wrappers;

    using LiteDB;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ShoutoutRepositoryTests
    {
        private Mock<ILiteCollection<Shoutout>> entities;

        private ShoutoutRepository repository;

        [SetUp]
        public void Setup()
        {
            Mock<IDatabaseFactory> databaseFactory = new Mock<IDatabaseFactory>();
            Mock<IDatabase> database = new Mock<IDatabase>();
            this.entities = new Mock<ILiteCollection<Shoutout>>();
            database.Setup(d => d.GetCollection<Shoutout>("shoutouts")).Returns(this.entities.Object);
            databaseFactory.Setup(f => f.Create()).Returns(database.Object);

            this.repository = new ShoutoutRepository(databaseFactory.Object);
        }

        [TestCase("key1")]
        [TestCase("key2")]
        public void GetShouldReturnElement(string id)
        {
            Shoutout entity = new Shoutout { Id = id };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<Shoutout, bool>>>())).Returns(entity);

            Shoutout shoutout = this.repository.Get(id);

            Assert.AreEqual(entity, shoutout);
        }

        [TestCase("key1")]
        [TestCase("key2")]
        public void AddOrUpdateShouldAddNewElement(string id)
        {
            Shoutout model = new Shoutout();

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model));
            this.entities.Verify(e => e.Update(model), Times.Never);
        }

        [TestCase("key1", "shoutoutText1")]
        [TestCase("key2", "shoutoutText2")]
        public void AddOrUpdateShouldUpdateElement(string id, string shoutoutText)
        {
            Shoutout entity = new Shoutout();
            Shoutout model = new Shoutout { ShoutoutText = shoutoutText };
            this.entities.Setup(e => e.FindOne(It.IsAny<Expression<Func<Shoutout, bool>>>())).Returns(entity);

            this.repository.AddOrUpdate(model);

            this.entities.Verify(e => e.Insert(model), Times.Never);
            this.entities.Verify(e => e.Update(entity));
            Assert.AreEqual(shoutoutText, entity.ShoutoutText);
        }
    }
}