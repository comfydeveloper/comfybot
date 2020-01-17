namespace ComfyBot.Application.Tests.Responses
{
    using System.Linq;

    using ComfyBot.Application.Responses;
    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class ResponseTabViewModelTests
    {
        private Mock<IRepository<MessageResponse>> repository;
        private Mock<IMapper<MessageResponse, MessageResponseModel>> mapper;

        private ResponseTabViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.repository = new Mock<IRepository<MessageResponse>>();
            this.mapper = new Mock<IMapper<MessageResponse, MessageResponseModel>>();

            this.viewModel = new ResponseTabViewModel(this.repository.Object, this.mapper.Object);
        }

        [Test]
        public void AddResponseCommandShouldAddResponse()
        {
            this.viewModel.AddResponseCommand.Execute();

            Assert.AreEqual(1, this.viewModel.Responses.Count);
        }

        [TestCase("00000000-0000-0000-0000-000000000000")]
        [TestCase("00000000-0000-0000-0000-000000000001")]
        public void RemoveResponseCommandShouldRemoveResponse(string id)
        {
            MessageResponseModel model = new MessageResponseModel { Id = id };
            this.viewModel.Responses.Add(model);

            this.viewModel.RemoveResponseCommand.Execute(model);

            Assert.AreEqual(0, this.viewModel.Responses.Count);
            this.repository.Verify(r => r.Remove(id));
        }

        [TestCase(5)]
        [TestCase(10)]
        public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
        {
            MessageResponse[] entities = Enumerable.Repeat(new MessageResponse(), count).ToArray();
            this.repository.Setup(r => r.GetAll()).Returns(entities);

            this.viewModel.IsSelected = true;
            this.viewModel.IsSelected = true;

            Assert.AreEqual(count, this.viewModel.Responses.Count);
            this.mapper.Verify(m => m.MapToModel(It.IsAny<MessageResponse>(), It.IsAny<MessageResponseModel>()), () => Times.Exactly(count));
        }

        [Test]
        public void UpdatingATextModelShouldUpdateEntity()
        {
            MessageResponseModel model = new MessageResponseModel();
            this.viewModel.Responses.Add(model);
            this.viewModel.IsSelected = true;

            model.Timeout = 1;

            this.repository.Verify(r => r.AddOrUpdate(It.IsAny<MessageResponse>()));
            this.mapper.Verify(r => r.MapToEntity(model, It.IsAny<MessageResponse>()));
        }
    }
}