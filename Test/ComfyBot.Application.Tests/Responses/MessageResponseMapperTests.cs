namespace ComfyBot.Application.Tests.Responses
{
    using System.Linq;

    using ComfyBot.Application.Responses;
    using ComfyBot.Application.Shared;
    using Data.Models;

    using NUnit.Framework;

    [TestFixture]
    public class MessageResponseMapperTests
    {
        private MessageResponse entity;
        private MessageResponseModel model;

        private MessageResponseMapper mapper;

        [SetUp]
        public void Setup()
        {
            entity = new MessageResponse();
            model = new MessageResponseModel();

            mapper = new MessageResponseMapper();
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void MapToEntityShouldMapId(string id)
        {
            model.Id = id;

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(id, entity.Id);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToEntityShouldMapTimeout(int timeout)
        {
            model.Timeout = timeout;

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(timeout, entity.TimeoutInSeconds);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToEntityShouldMapPriority(int priority)
        {
            model.Priority = priority;

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(priority, entity.Priority);
        }

        [TestCase("user1")]
        [TestCase("user2")]
        public void MapToEntityShouldMapUsers(string userName)
        {
            model.Users.Add(new TextModel { Text = userName });

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(1, entity.Users.Count);
            Assert.AreEqual(userName, entity.Users.First());
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToEntityShouldMapLooseKeywords(string keyword)
        {
            model.LooseKeywords.Add(new TextModel { Text = keyword });

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(1, entity.LooseKeywords.Count);
            Assert.AreEqual(keyword, entity.LooseKeywords.First());
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToEntityShouldMapExactKeywords(string keyword)
        {
            model.ExactKeywords.Add(new TextModel { Text = keyword });

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(1, entity.ExactKeywords.Count);
            Assert.AreEqual(keyword, entity.ExactKeywords.First());
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToEntityShouldMapAllKeywords(string keyword)
        {
            model.AllKeywords.Add(new TextModel { Text = keyword });

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(1, entity.AllKeywords.Count);
            Assert.AreEqual(keyword, entity.AllKeywords.First());
        }

        [TestCase("reply1")]
        [TestCase("reply2")]
        public void MapToEntityShouldMapReplies(string reply)
        {
            model.Replies.Add(new TextModel { Text = reply });

            mapper.MapToEntity(model, entity);

            Assert.AreEqual(1, entity.Replies.Count);
            Assert.AreEqual(reply, entity.Replies.First());
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void MapToModelShouldMapId(string id)
        {
            entity.Id = id;

            mapper.MapToModel(entity, model);

            Assert.AreEqual(id, model.Id);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToModelShouldMapTimeout(int timeout)
        {
            entity.TimeoutInSeconds = timeout;

            mapper.MapToModel(entity, model);

            Assert.AreEqual(timeout, model.Timeout);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToModelShouldMapPriority(int priority)
        {
            entity.Priority = priority;

            mapper.MapToModel(entity, model);

            Assert.AreEqual(priority, model.Priority);
        }

        [TestCase("user1")]
        [TestCase("user2")]
        public void MapToModelShouldMapUsers(string userName)
        {
            entity.Users.Add(userName);

            mapper.MapToModel(entity, model);

            Assert.AreEqual(1, model.Users.Count);
            Assert.AreEqual(userName, model.Users.First().Text);
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToModelShouldMapLooseKeywords(string keyword)
        {
            entity.LooseKeywords.Add(keyword);

            mapper.MapToModel(entity, model);

            Assert.AreEqual(1, model.LooseKeywords.Count);
            Assert.AreEqual(keyword, model.LooseKeywords.First().Text);
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToModelShouldMapExactKeywords(string keyword)
        {
            entity.ExactKeywords.Add(keyword);

            mapper.MapToModel(entity, model);

            Assert.AreEqual(1, model.ExactKeywords.Count);
            Assert.AreEqual(keyword, model.ExactKeywords.First().Text);
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToModelShouldMapAllKeywords(string keyword)
        {
            entity.AllKeywords.Add(keyword);

            mapper.MapToModel(entity, model);

            Assert.AreEqual(1, model.AllKeywords.Count);
            Assert.AreEqual(keyword, model.AllKeywords.First().Text);
        }

        [TestCase("reply1")]
        [TestCase("reply2")]
        public void MapToModelShouldMapReplies(string reply)
        {
            entity.Replies.Add(reply);

            mapper.MapToModel(entity, model);

            Assert.AreEqual(1, model.Replies.Count);
            Assert.AreEqual(reply, model.Replies.First().Text);
        }

        [Test]
        public void MapToModelShouldOrderReplies()
        {
            entity.Replies.Add("B");
            entity.Replies.Add("C");
            entity.Replies.Add("A");

            mapper.MapToModel(entity, model);

            string[] replies = model.Replies.Select(r => r.Text).ToArray();
            Assert.AreEqual("A", replies[0]);
            Assert.AreEqual("B", replies[1]);
            Assert.AreEqual("C", replies[2]);
        }

        [Test]
        public void MapToModelShouldOrderAllKeywords()
        {
            entity.AllKeywords.Add("B");
            entity.AllKeywords.Add("C");
            entity.AllKeywords.Add("A");

            mapper.MapToModel(entity, model);

            string[] keywords = model.AllKeywords.Select(r => r.Text).ToArray();
            Assert.AreEqual("A", keywords[0]);
            Assert.AreEqual("B", keywords[1]);
            Assert.AreEqual("C", keywords[2]);
        }

        [Test]
        public void MapToModelShouldOrderLooseKeywords()
        {
            entity.LooseKeywords.Add("B");
            entity.LooseKeywords.Add("C");
            entity.LooseKeywords.Add("A");

            mapper.MapToModel(entity, model);

            string[] keywords = model.LooseKeywords.Select(r => r.Text).ToArray();
            Assert.AreEqual("A", keywords[0]);
            Assert.AreEqual("B", keywords[1]);
            Assert.AreEqual("C", keywords[2]);
        }

        [Test]
        public void MapToModelShouldOrderExactKeywords()
        {
            entity.ExactKeywords.Add("B");
            entity.ExactKeywords.Add("C");
            entity.ExactKeywords.Add("A");

            mapper.MapToModel(entity, model);

            string[] keywords = model.ExactKeywords.Select(r => r.Text).ToArray();
            Assert.AreEqual("A", keywords[0]);
            Assert.AreEqual("B", keywords[1]);
            Assert.AreEqual("C", keywords[2]);
        }
    }
}