namespace ComfyBot.Application.Tests.Shoutouts
{
    using ComfyBot.Application.Shoutouts;
    using ComfyBot.Data.Models;

    using NUnit.Framework;

    [TestFixture]
    public class ShoutoutMapperTests
    {
        private Shoutout entity;
        private ShoutoutModel model;

        private ShoutoutMapper mapper;

        [SetUp]
        public void Setup()
        {
            this.entity = new Shoutout();
            this.model = new ShoutoutModel();

            this.mapper = new ShoutoutMapper();
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void MapToEntityShouldMapGuid(string guid)
        {
            this.model.Id = guid;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(guid, this.entity.Id);
        }

        [TestCase("command1")]
        [TestCase("command2")]
        public void MapToEntityShouldMapCommand(string command)
        {
            this.model.Command = command;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(command, this.entity.Command);
        }

        [TestCase("message1")]
        [TestCase("message2")]
        public void MapToEntityShouldMapMessage(string message)
        {
            this.model.Message = message;

            this.mapper.MapToEntity(this.model, this.entity);

            Assert.AreEqual(message, this.entity.Message);
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void MapToModelShouldMapGuid(string guid)
        {
            this.entity.Id = guid;

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(guid, this.model.Id);
        }

        [TestCase("command1")]
        [TestCase("command2")]
        public void MapToModelShouldMapCommand(string command)
        {
            this.entity.Command = command;

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(command, this.model.Command);
        }

        [TestCase("message1")]
        [TestCase("message2")]
        public void MapToModelShouldMapMessage(string message)
        {
            this.entity.Message = message;

            this.mapper.MapToModel(this.entity, this.model);

            Assert.AreEqual(message, this.model.Message);
        }
    }
}