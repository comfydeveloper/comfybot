namespace ComfyBot.Application.Tests.TextCommands
{
    using System.Linq;
    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.TextCommands;
    using Data.Models;
    using Data.Repositories;

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
            repository = new Mock<IRepository<TextCommand>>();
            mapper = new Mock<IMapper<TextCommand, TextCommandModel>>();

            viewModel = new TextCommandsTabViewModel(repository.Object, mapper.Object);
        }

        [Test]
        public void AddTextCommandCommandShouldAddNewTextCommand()
        {
            viewModel.AddTextCommandCommand.Execute();

            Assert.AreEqual(1, viewModel.Commands.Count);
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void RemoveTextCommandCommandShouldRemoveResponse(string id)
        {
            TextCommandModel model = new TextCommandModel { Id = id };
            viewModel.Commands.Add(model);

            viewModel.RemoveTextCommandCommand.Execute(model);

            Assert.AreEqual(0, viewModel.Commands.Count);
            repository.Verify(r => r.Remove(id));
        }

        [TestCase(5)]
        [TestCase(10)]
        public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
        {
            TextCommand[] entities = Enumerable.Repeat(new TextCommand(), count).ToArray();
            repository.Setup(r => r.GetAll()).Returns(entities);

            viewModel.IsSelected = true;
            viewModel.IsSelected = true;

            Assert.AreEqual(count, viewModel.Commands.Count);
            mapper.Verify(m => m.MapToModel(It.IsAny<TextCommand>(), It.IsAny<TextCommandModel>()), () => Times.Exactly(count));
        }

        [Test]
        public void UpdatingATextModelShouldUpdateEntity()
        {
            TextCommandModel model = new TextCommandModel();
            viewModel.Commands.Add(model);
            viewModel.IsSelected = true;

            model.Timeout = 1;

            repository.Verify(r => r.AddOrUpdate(It.IsAny<TextCommand>()));
            mapper.Verify(r => r.MapToEntity(model, It.IsAny<TextCommand>()));
        }
    }
}