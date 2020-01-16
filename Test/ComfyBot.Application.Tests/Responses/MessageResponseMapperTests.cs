namespace ComfyBot.Application.Tests.Responses
{
    using System.Linq;

    using ComfyBot.Application.Responses;
    using ComfyBot.Data.Models;

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
            this.entity = new MessageResponse();
            this.model = new MessageResponseModel();

            this.mapper = new MessageResponseMapper();
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void MapToEntityShouldMapId(string id)
        {
            this.model.Id = id;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(id, this.entity.Id);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToEntityShouldMapTimeout(int timeout)
        {
            this.model.Timeout = timeout;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(timeout, this.entity.TimeoutInSeconds);
        }

        [TestCase("user1")]
        [TestCase("user2")]
        public void MapToEntityShouldMapUsers(string userName)
        {
            this.model.Users.Add(new TextModel { Text = userName });

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(1, this.entity.Users.Count);
            Assert.AreEqual(userName, this.entity.Users.First());
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToEntityShouldMapLooseKeywords(string keyword)
        {
            this.model.LooseKeywords.Add(new TextModel { Text = keyword });

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(1, this.entity.LooseKeywords.Count);
            Assert.AreEqual(keyword, this.entity.LooseKeywords.First());
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToEntityShouldMapExactKeywords(string keyword)
        {
            this.model.ExactKeywords.Add(new TextModel { Text = keyword });

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(1, this.entity.ExactKeywords.Count);
            Assert.AreEqual(keyword, this.entity.ExactKeywords.First());
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToEntityShouldMapAllKeywords(string keyword)
        {
            this.model.AllKeywords.Add(new TextModel { Text = keyword });

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(1, this.entity.AllKeywords.Count);
            Assert.AreEqual(keyword, this.entity.AllKeywords.First());
        }

        [TestCase("reply1")]
        [TestCase("reply2")]
        public void MapToEntityShouldMapReplies(string reply)
        {
            this.model.Replies.Add(new TextModel { Text = reply });

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(1, this.entity.Replies.Count);
            Assert.AreEqual(reply, this.entity.Replies.First());
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void MapToModelShouldMapId(string id)
        {
            this.entity.Id = id;

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(id, this.model.Id);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToModelShouldMapTimeout(int timeout)
        {
            this.entity.TimeoutInSeconds = timeout;

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(timeout, this.model.Timeout);
        }

        [TestCase("user1")]
        [TestCase("user2")]
        public void MapToModelShouldMapUsers(string userName)
        {
            this.entity.Users.Add(userName);

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(1, this.model.Users.Count);
            Assert.AreEqual(userName, this.model.Users.First().Text);
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToModelShouldMapLooseKeywords(string keyword)
        {
            this.entity.LooseKeywords.Add(keyword);

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(1, this.model.LooseKeywords.Count);
            Assert.AreEqual(keyword, this.model.LooseKeywords.First().Text);
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToModelShouldMapExactKeywords(string keyword)
        {
            this.entity.ExactKeywords.Add(keyword);

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(1, this.model.ExactKeywords.Count);
            Assert.AreEqual(keyword, this.model.ExactKeywords.First().Text);
        }

        [TestCase("keyword1")]
        [TestCase("keyword2")]
        public void MapToModelShouldMapAllKeywords(string keyword)
        {
            this.entity.AllKeywords.Add(keyword);

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(1, this.model.AllKeywords.Count);
            Assert.AreEqual(keyword, this.model.AllKeywords.First().Text);
        }

        [TestCase("reply1")]
        [TestCase("reply2")]
        public void MapToModelShouldMapReplies(string reply)
        {
            this.entity.Replies.Add(reply);

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(1, this.model.Replies.Count);
            Assert.AreEqual(reply, this.model.Replies.First().Text);
        }
    }
}