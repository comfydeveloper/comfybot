namespace ComfyBot.Application.Tests.TextCommands
{
    using System.Linq;

    using ComfyBot.Application.Responses;
    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.TextCommands;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class TextCommandsTabViewModelTests
    {
        private Mock<IRepository<TextCommand>> repository;
        private Mock<IMapper<TextCommand, TextCommandModel>> mapper;

        private TextCommandsTabViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<IRepository<TextCommand>>();
            this.mapper = new Mock<IMapper<TextCommand, TextCommandModel>>();

            this.viewModel = new TextCommandsTabViewModel(this.repository.Object, this.mapper.Object);
        }

        [Test]
        public void AddTextCommandCommandShouldAddNewTextCommand()
        {
            this.viewModel.AddTextCommandCommand.Execute();

            Assert.AreEqual(1, this.viewModel.Commands.Count);
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void RemoveTextCommandCommandShouldRemoveResponse(string id)
        {
            TextCommandModel model = new TextCommandModel { Id = id };
            this.viewModel.Commands.Add(model);

            this.viewModel.RemoveTextCommandCommand.Execute(model);

            Assert.AreEqual(0, this.viewModel.Commands.Count);
            this.repository.Verify(r => r.Remove(id));
        }

        [TestCase(5)]
        [TestCase(10)]
        public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
        {
            TextCommand[] entities = Enumerable.Repeat(new TextCommand(), count).ToArray();
            this.repository.Setup(r => r.GetAll()).Returns(entities);

            this.viewModel.IsSelected = true;
            this.viewModel.IsSelected = true;

            Assert.AreEqual(count, this.viewModel.Commands.Count);
            this.mapper.Verify(m => m.MapToModel(It.IsAny<TextCommand>(), It.IsAny<TextCommandModel>()), () => Times.Exactly(count));
        }

        [Test]
        public void UpdatingATextModelShouldUpdateEntity()
        {
            TextCommandModel model = new TextCommandModel();
            this.viewModel.Commands.Add(model);
            this.viewModel.IsSelected = true;

            model.Timeout = 1;

            this.repository.Verify(r => r.AddOrUpdate(It.IsAny<TextCommand>()));
            this.mapper.Verify(r => r.MapToEntity(model, It.IsAny<TextCommand>()));
        }
    }
}