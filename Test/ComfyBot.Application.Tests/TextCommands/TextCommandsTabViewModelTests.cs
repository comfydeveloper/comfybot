using System.Linq;
using System.Windows;
using ComfyBot.Application.Shared.Contracts;
using ComfyBot.Application.Shared.Wrappers;
using ComfyBot.Application.TextCommands;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Moq;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.TextCommands;

[TestFixture]
public class TextCommandsTabViewModelTests
{
    private Mock<IRepository<TextCommand>> repository;
    private Mock<IMapper<TextCommand, TextCommandModel>> mapper;
    private Mock<IMessageBox> messageBox;
        
    private TextCommandsTabViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        repository = new Mock<IRepository<TextCommand>>();
        mapper = new Mock<IMapper<TextCommand, TextCommandModel>>();
        messageBox = new Mock<IMessageBox>();


        viewModel = new TextCommandsTabViewModel(repository.Object, mapper.Object, messageBox.Object);
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
        messageBox.Setup(b => b.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>())).Returns(MessageBoxResult.Yes);

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

        repository.Verify(r => r.Write(It.IsAny<TextCommand>()));
        mapper.Verify(r => r.MapToEntity(model, It.IsAny<TextCommand>()));
    }
}