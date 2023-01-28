namespace ComfyBot.Application.Tests.Responses;

using System.Linq;

using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared.Contracts;
using Data.Models;
using Data.Repositories;

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
        repository = new Mock<IRepository<MessageResponse>>();
        mapper = new Mock<IMapper<MessageResponse, MessageResponseModel>>();

        viewModel = new ResponseTabViewModel(repository.Object, mapper.Object);
    }

    [Test]
    public void AddResponseCommandShouldAddResponse()
    {
        viewModel.AddResponseCommand.Execute();

        Assert.AreEqual(1, viewModel.Responses.Count);
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase("00000000-0000-0000-0000-000000000001")]
    public void RemoveResponseCommandShouldRemoveResponse(string id)
    {
        MessageResponseModel model = new MessageResponseModel { Id = id };
        viewModel.Responses.Add(model);

        viewModel.RemoveResponseCommand.Execute(model);

        Assert.AreEqual(0, viewModel.Responses.Count);
        repository.Verify(r => r.Remove(id));
    }

    [TestCase(5)]
    [TestCase(10)]
    public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
    {
        MessageResponse[] entities = Enumerable.Repeat(new MessageResponse(), count).ToArray();
        repository.Setup(r => r.GetAll()).Returns(entities);

        viewModel.IsSelected = true;
        viewModel.IsSelected = true;

        Assert.AreEqual(count, viewModel.Responses.Count);
        mapper.Verify(m => m.MapToModel(It.IsAny<MessageResponse>(), It.IsAny<MessageResponseModel>()), () => Times.Exactly(count));
    }

    [Test]
    public void UpdatingATextModelShouldUpdateEntity()
    {
        MessageResponseModel model = new MessageResponseModel();
        viewModel.Responses.Add(model);
        viewModel.IsSelected = true;

        model.Timeout = 1;

        repository.Verify(r => r.Write(It.IsAny<MessageResponse>()));
        mapper.Verify(r => r.MapToEntity(model, It.IsAny<MessageResponse>()));
    }
}