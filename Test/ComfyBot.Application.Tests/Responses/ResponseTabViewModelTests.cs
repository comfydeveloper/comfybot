using System.Linq;
using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.Responses;

[TestFixture]
public class ResponseTabViewModelTests
{
    private IQueryableRepository repository;
    private Mock<IMapper<MessageResponse, MessageResponseModel>> mapper;

    private ResponseTabViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        this.repository = Substitute.For<IQueryableRepository>();
        this.mapper = new Mock<IMapper<MessageResponse, MessageResponseModel>>();

        this.viewModel = new ResponseTabViewModel(this.repository, this.mapper.Object);
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
        MessageResponseModel model = new() { Id = id };
        this.viewModel.Responses.Add(model);

        this.viewModel.RemoveResponseCommand.Execute(model);

        Assert.AreEqual(0, this.viewModel.Responses.Count);
        this.repository.Verify(r => r.Remove(id));
    }

    [TestCase(5)]
    [TestCase(10)]
    public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
    {
        MessageResponseOld[] entities = Enumerable.Repeat(new MessageResponseOld(), count).ToArray();
        this.repository.Setup(r => r.GetAll()).Returns(entities);

        this.viewModel.IsSelected = true;
        this.viewModel.IsSelected = true;

        Assert.AreEqual(count, this.viewModel.Responses.Count);
        this.mapper.Verify(m => m.MapToModel(It.IsAny<MessageResponseOld>(), It.IsAny<MessageResponseModel>()), () => Times.Exactly(count));
    }

    [Test]
    public void UpdatingATextModelShouldUpdateEntity()
    {
        MessageResponseModel model = new();
        this.viewModel.Responses.Add(model);
        this.viewModel.IsSelected = true;

        model.TimeoutInSeconds = 1;

        this.repository.Verify(r => r.Write(It.IsAny<MessageResponseOld>()));
        this.mapper.Verify(r => r.MapToEntity(model, It.IsAny<MessageResponseOld>()));
    }
}