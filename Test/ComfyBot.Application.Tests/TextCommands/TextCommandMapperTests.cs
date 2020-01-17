namespace ComfyBot.Application.Tests.TextCommands
{
    using System.Linq;

    using ComfyBot.Application.Responses;
    using ComfyBot.Application.Shared;
    using ComfyBot.Application.TextCommands;
    using ComfyBot.Data.Models;

    using NUnit.Framework;

    [TestFixture]
    public class TextCommandMapperTests
    {
        private TextCommand entity;
        private TextCommandModel model;

        private TextCommandMapper mapper;

        [SetUp]
        public void Setup()
        {
            this.entity = new TextCommand();
            this.model = new TextCommandModel();

            this.mapper = new TextCommandMapper();
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void MapToEntityShouldMapId(string id)
        {
            this.model.Id = id;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(id, this.entity.Id);
        }

        [TestCase("command1")]
        [TestCase("command2")]
        public void MapToEntityShouldMapCommand(string command)
        {
            this.model.Command = command;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(command, this.entity.Command);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToEntityShouldMapTimeout(int timeout)
        {
            this.model.Timeout = timeout;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(timeout, this.entity.TimeoutInSeconds);
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

        [TestCase("command1")]
        [TestCase("command2")]
        public void MapToModelShouldMapCommand(string command)
        {
            this.entity.Command = command;

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(command, this.model.Command);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void MapToModelShouldMapTimeout(int timeout)
        {
            this.entity.TimeoutInSeconds = timeout;

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(timeout, this.model.Timeout);
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