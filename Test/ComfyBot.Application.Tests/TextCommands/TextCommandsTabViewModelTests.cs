using System;
using System.Linq;
using System.Windows;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Features.TextCommands;
using ComfyBot.Application.Shared.Wrappers;
using ComfyBot.Application.TextCommands;
using ComfyBot.Application.Variables;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace ComfyBot.Application.Tests.TextCommands;

[TestFixture]
public class TextCommandsTabViewModelTests
{
    private IQueryHandler<GetCommands.Query, GetCommands.Result> getHandler;
    private ICommandHandler<AddCommand.Command> addHandler;
    private ICommandHandler<UpdateCommand.Command> updateHandler;
    private ICommandHandler<RemoveCommand.Command> removeHandler;
    private IMessageBox messageBox;
        
    private TextCommandsTabViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        this.getHandler = Substitute.For<IQueryHandler<GetCommands.Query, GetCommands.Result>>();
        this.addHandler = Substitute.For<ICommandHandler<AddCommand.Command>>();
        this.updateHandler = Substitute.For<ICommandHandler<UpdateCommand.Command>>();
        this.removeHandler = Substitute.For<ICommandHandler<RemoveCommand.Command>>();
        this.messageBox = Substitute.For<IMessageBox>();


        this.viewModel = new TextCommandsTabViewModel(this.getHandler, this.addHandler, this.updateHandler, this.removeHandler, this.messageBox);
    }

    [Test]
    public void AddTextCommandCommandShouldAddNewTextCommand()
    {
        this.viewModel.AddTextCommandCommand.Execute();

        this.addHandler.Received(1).Handle(Arg.Any<AddCommand.Command>());
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase("00000000-0000-0000-0000-000000000001")]
    public void RemoveTextCommandCommandShouldRemoveResponse(string id)
    {
        TextCommandModel model = new() { Id = id };
        this.viewModel.Commands.Add(model);
        this.messageBox.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>()).Returns(MessageBoxResult.Yes);


        this.viewModel.RemoveTextCommandCommand.Execute(model);

        this.viewModel.Commands.Should().BeEmpty();
        this.removeHandler.Received(1).Handle(Arg.Is<RemoveCommand.Command>(x => x.Id == Guid.Parse(id)));
    }

    [TestCase(5)]
    [TestCase(10)]
    public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
    {
        GetCommands.TextCommandEntry[] entries = Enumerable.Repeat(CreateTextCommandEntry(), count).ToArray();
        this.getHandler.Handle(default).ReturnsForAnyArgs(new GetCommands.Result { Entries = entries.ToList() });

        this.viewModel.IsSelected = true;
        this.viewModel.IsSelected = true;

        this.viewModel.Commands.Count.Should().Be(count);
    }

    [Test]
    public void UpdatingATextModelShouldUpdateEntity()
    {
        TextCommandModel model = new() { Id = Guid.NewGuid().ToString() };
        this.getHandler.Handle(default).ReturnsForAnyArgs(new GetCommands.Result());
        this.viewModel.Commands.Add(model);
        this.viewModel.IsSelected = true;

        model.Timeout = 1;

        this.updateHandler.Received(1).Handle(Arg.Any<UpdateCommand.Command>());
    }

    private static GetCommands.TextCommandEntry CreateTextCommandEntry()
    {
        return new GetCommands.TextCommandEntry(default, default, [], []);
    }
}