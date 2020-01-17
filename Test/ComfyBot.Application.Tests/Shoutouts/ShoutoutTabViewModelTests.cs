namespace ComfyBot.Application.Tests.Shoutouts
{
    using System.Linq;

    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.Shoutouts;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    using Moq;

    using NUnit.Framework;

    public class ShoutoutTabViewModelTests
    {
        private Mock<IRepository<Shoutout>> repository;
        private Mock<IMapper<Shoutout, ShoutoutModel>> mapper;

        private ShoutoutTabViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<IRepository<Shoutout>>();
            this.mapper = new Mock<IMapper<Shoutout, ShoutoutModel>>();

            this.viewModel = new ShoutoutTabViewModel(this.repository.Object, this.mapper.Object);
        }

        [Test]
        public void AddShoutoutCommandShouldAddShoutout()
        {
            this.viewModel.AddShoutoutCommand.Execute();

            Assert.AreEqual(1, this.viewModel.Shoutouts.Count);
        }

        [Test]
        public void RemoveShoutoutCommandShouldRemoveShoutout()
        {
            ShoutoutModel model = new ShoutoutModel();
            this.viewModel.Shoutouts.Add(model);

            this.viewModel.RemoveShoutoutCommand.Execute(model);

            Assert.AreEqual(0, this.viewModel.Shoutouts.Count);
        }

        [TestCase(5)]
        [TestCase(10)]
        public void IsSelectedSetterShouldInitializeDataFromRepositoryOnce(int count)
        {
            Shoutout[] entities = Enumerable.Repeat(new Shoutout(), count).ToArray();
            this.repository.Setup(r => r.GetAll()).Returns(entities);

            this.viewModel.IsSelected = true;
            this.viewModel.IsSelected = true;

            Assert.AreEqual(count, this.viewModel.Shoutouts.Count);
            this.mapper.Verify(m => m.MapToModel(It.IsAny<Shoutout>(), It.IsAny<ShoutoutModel>()), () => Times.Exactly(count));
            Assert.AreEqual(true, this.viewModel.IsSelected);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("a", true)]
        public void UpdatingAShoutoutShouldDelegateToRepository(string shoutoutText, bool expected)
        {
            ShoutoutModel model = new ShoutoutModel { Command = "a" };
            this.viewModel.Shoutouts.Add(model);
            this.viewModel.IsSelected = true;

            model.Message = shoutoutText;

            this.repository.Verify(r => r.AddOrUpdate(It.IsAny<Shoutout>()), () => expected ? Times.Once() : Times.Never());
            this.mapper.Verify(m => m.MapToEntity(It.IsAny<ShoutoutModel>(), It.IsAny<Shoutout>()), () => expected ? Times.Once() : Times.Never());
        }
    }
}