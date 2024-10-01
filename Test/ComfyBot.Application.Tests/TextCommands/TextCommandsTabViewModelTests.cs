using System.Linq;
using System.Windows;
using ComfyBot.Application.Shared.Contracts;
using ComfyBot.Application.Shared.Wrappers;
using ComfyBot.Application.TextCommands;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.TextCommands;

[TestFixture]
public class TextCommandsTabViewModelTests
{
    private Mock<IRepository<TextCommandOld>> repository;
    private Mock<IMapper<TextCommandOld, TextCommandModel>> mapper;
    private Mock<IMessageBox> messageBox;
        
    private TextCommandsTabViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        this.repository = new Mock<IRepository<TextCommandOld>>();
        this.mapper = new Mock<IMapper<TextCommandOld, TextCommandModel>>();
        this.messageBox = new Mock<IMessageBox>();


        this.viewModel = new TextCommandsTabViewModel(this.repository.Object, this.mapper.Object, this.messageBox.Object);
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
        TextCommandModel model = new() { Id = id };
        this.viewModel.Commands.Add(model);
        this.messageBox.Setup(b => b.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>())).Returns(MessageBoxResult.Yes);

        this.viewModel.RemoveTextCommandCommand.Execute(model);

        Assert.AreEqual(0, this.viewModel.Commands.Count);
        this.repository.Verify(r => r.Remove(id));
    }

    [TestCase(5)]
    [TestCase(10)]
    public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
    {
        TextCommandOld[] entities = Enumerable.Repeat(new TextCommandOld(), count).ToArray();
        this.repository.Setup(r => r.GetAll()).Returns(entities);

        this.viewModel.IsSelected = true;
        this.viewModel.IsSelected = true;

        Assert.AreEqual(count, this.viewModel.Commands.Count);
        this.mapper.Verify(m => m.MapToModel(It.IsAny<TextCommandOld>(), It.IsAny<TextCommandModel>()), () => Times.Exactly(count));
    }

    [Test]
    public void UpdatingATextModelShouldUpdateEntity()
    {
        TextCommandModel model = new();
        this.viewModel.Commands.Add(model);
        this.viewModel.IsSelected = true;

        model.Timeout = 1;

        this.repository.Verify(r => r.Write(It.IsAny<TextCommandOld>()));
        this.mapper.Verify(r => r.MapToEntity(model, It.IsAny<TextCommandOld>()));
    }
}