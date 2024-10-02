using ComfyBot.Application.Features.MessageResponses;
using ComfyBot.Application.Features.Shared.Contracts;
using System.Linq;
using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared.Wrappers;
using NSubstitute;
using NUnit.Framework;
using System;

namespace ComfyBot.Application.Tests.Responses;

[TestFixture]
public class ResponseTabViewModelTests
{
    private IQueryHandler<GetResponses.Query, GetResponses.Result> getHandler;
    private ICommandHandler<AddResponse.Command> addHandler;
    private ICommandHandler<UpdateResponse.Command> updateHandler;
    private ICommandHandler<RemoveResponse.Command> removeHandler;
    private IMessageBox messageBox;

    private ResponseTabViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        this.getHandler = Substitute.For<IQueryHandler<GetResponses.Query, GetResponses.Result>>();
        this.addHandler = Substitute.For<ICommandHandler<AddResponse.Command>>();
        this.updateHandler = Substitute.For<ICommandHandler<UpdateResponse.Command>>();
        this.removeHandler = Substitute.For<ICommandHandler<RemoveResponse.Command>>();
        this.messageBox = Substitute.For<IMessageBox>();

        this.viewModel = new ResponseTabViewModel(this.getHandler, this.addHandler, this.updateHandler, this.removeHandler, this.messageBox);
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
        this.removeHandler.Received(1).Handle(Arg.Is<RemoveResponse.Command>(x => x.Id == Guid.Parse(id)));
    }

    [TestCase(5)]
    [TestCase(10)]
    public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
    {
        GetResponses.MessageResponseEntry[] entries = Enumerable.Repeat(CreateResponseEntry(), count).ToArray();
        this.getHandler.Handle(default).ReturnsForAnyArgs(new GetResponses.Result { Entries = entries.ToList()});

        this.viewModel.IsSelected = true;
        this.viewModel.IsSelected = true;

        Assert.AreEqual(count, this.viewModel.Responses.Count);
    }

    private static GetResponses.MessageResponseEntry CreateResponseEntry()
    {
        return new GetResponses.MessageResponseEntry(default, default, default, default, default, default, default, default, default);
    }

    [Test]
    public void UpdatingATextModelShouldUpdateEntity()
    {
        MessageResponseModel model = new();
        this.viewModel.Responses.Add(model);
        this.viewModel.IsSelected = true;

        model.TimeoutInSeconds = 1;

        this.updateHandler.Received(1).Handle(Arg.Any<UpdateResponse.Command>());
    }
}