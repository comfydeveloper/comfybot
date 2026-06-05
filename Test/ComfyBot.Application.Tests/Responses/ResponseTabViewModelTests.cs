using ComfyBot.Application.Features.MessageResponses;
using ComfyBot.Application.Features.Shared.Contracts;
using System.Linq;
using ComfyBot.Application.Patterns.Outcomes;
using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared.Services;
using ComfyBot.Application.Shared.Wrappers;
using NSubstitute;
using NUnit.Framework;
using Shouldly;
using System;
using System.Windows;

namespace ComfyBot.Application.Tests.Responses;

[TestFixture]
public class ResponseTabViewModelTests
{
    private IQueryHandler<GetResponses.Query, GetResponses.Result> getHandler;
    private ICommandHandler<AddResponse.Command> addHandler;
    private ICommandHandler<UpdateResponse.Command> updateHandler;
    private ICommandHandler<RemoveResponse.Command> removeHandler;
    private IScopedServiceProvider provider;
    private IMessageBox messageBox;

    private ResponseTabViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        this.getHandler = Substitute.For<IQueryHandler<GetResponses.Query, GetResponses.Result>>();
        this.addHandler = Substitute.For<ICommandHandler<AddResponse.Command>>();
        this.updateHandler = Substitute.For<ICommandHandler<UpdateResponse.Command>>();
        this.removeHandler = Substitute.For<ICommandHandler<RemoveResponse.Command>>();
        this.provider = Substitute.For<IScopedServiceProvider>();
        this.messageBox = Substitute.For<IMessageBox>();

        this.provider.Create<IQueryHandler<GetResponses.Query, GetResponses.Result>>().Returns(new ScopedService<IQueryHandler<GetResponses.Query, GetResponses.Result>>(null, this.getHandler));
        this.provider.Create<ICommandHandler<AddResponse.Command>>().Returns(new ScopedService<ICommandHandler<AddResponse.Command>>(null, this.addHandler));
        this.provider.Create<ICommandHandler<UpdateResponse.Command>>().Returns(new ScopedService<ICommandHandler<UpdateResponse.Command>>(null, this.updateHandler));
        this.provider.Create<ICommandHandler<RemoveResponse.Command>>().Returns(new ScopedService<ICommandHandler<RemoveResponse.Command>>(null, this.removeHandler));

        this.viewModel = new ResponseTabViewModel(this.provider, this.messageBox);
    }

    [Test]
    public void AddResponseCommandShouldAddResponse()
    {
        this.viewModel.AddResponseCommand.Execute();

        this.viewModel.Responses.Count.ShouldBe(1);
    }

    [TestCase("00000000-0000-0000-0000-000000000000")]
    [TestCase("00000000-0000-0000-0000-000000000001")]
    public void RemoveResponseCommandShouldRemoveResponse(string id)
    {
        MessageResponseModel model = new() { Id = id };
        this.viewModel.Responses.Add(model);
        this.messageBox.Show(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>()).Returns(MessageBoxResult.Yes);

        this.viewModel.RemoveResponseCommand.Execute(model);

        this.viewModel.Responses.ShouldBeEmpty();
        this.removeHandler.Received(1).Handle(Arg.Is<RemoveResponse.Command>(x => x.Id == Guid.Parse(id)));
    }

    [TestCase(5)]
    [TestCase(10)]
    public void IsSelectedSetterShouldInitializeFromRepositoryOnce(int count)
    {
        GetResponses.MessageResponseEntry[] entries = Enumerable.Repeat(CreateResponseEntry(), count).ToArray();
        GetResponses.Result result = new() { Entries = entries.ToList() };
        this.getHandler.Handle(default).ReturnsForAnyArgs(Outcome<GetResponses.Result>.Success(result));

        this.viewModel.IsSelected = true;
        this.viewModel.IsSelected = true;

        this.viewModel.Responses.Count.ShouldBe(count);
    }

    private static GetResponses.MessageResponseEntry CreateResponseEntry()
    {
        return new GetResponses.MessageResponseEntry(default, default, default, default, [], [], [], [], []);
    }

    [Test]
    public void UpdatingATextModelShouldUpdateEntity()
    {
        MessageResponseModel model = new () { Id = Guid.NewGuid().ToString() };
        GetResponses.Result result = new();
        this.getHandler.Handle(default).ReturnsForAnyArgs(Outcome<GetResponses.Result>.Success(result));
        this.viewModel.Responses.Add(model);
        this.viewModel.IsSelected = true;

        model.TimeoutInSeconds = 1;

        this.updateHandler.Received(1).Handle(Arg.Any<UpdateResponse.Command>());
    }
}