using ComfyBot.Application.Features.MessageResponses;
using ComfyBot.Application.Features.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Data;
using ComfyBot.Application.Shared;
using ComfyBot.Application.Shared.Contracts;
using ComfyBot.Application.Shared.Extensions;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;

namespace ComfyBot.Application.Responses;

public class ResponseTabViewModel : InitializableTab
{
    private readonly IQueryHandler<GetResponses.Query, GetResponses.Result> getHandler;
    private readonly ICommandHandler<AddResponse.Command> addHandler;
    private readonly ICommandHandler<UpdateResponse.Command> updateHandler;
    private readonly ICommandHandler<RemoveResponse.Command> removeHandler;

    private string searchText;

    public ResponseTabViewModel(
        IQueryHandler<GetResponses.Query, GetResponses.Result> getHandler,
        ICommandHandler<AddResponse.Command> addHandler,
        ICommandHandler<UpdateResponse.Command> updateHandler,
        ICommandHandler<RemoveResponse.Command> removeHandler)
    {
        this.getHandler = getHandler;
        this.addHandler = addHandler;
        this.updateHandler = updateHandler;
        this.removeHandler = removeHandler;

        this.AddResponseCommand = new DelegateCommand(this.AddResponse);
        this.RemoveResponseCommand = new ParameterCommand(this.RemoveResponse);
    }

    public DelegateCommand AddResponseCommand { get; }

    public ParameterCommand RemoveResponseCommand { get; }

    public ObservableCollection<MessageResponseModel> Responses { get; set; } = [];

    protected override void Initialize()
    {
        GetResponses.Result result = this.getHandler.Handle(new GetResponses.Query()).Result;

        foreach (GetResponses.MessageResponseEntry entry in result.Entries)
        {
            MessageResponseModel model = new()
            {
                Id = entry.Id.ToString(),
                TimeoutInSeconds = entry.TimeoutInSeconds,
                Priority = entry.Priority,
                ReplyAlways = entry.AlwaysReply,
            };

            model.Users.AddRange(entry.Users.ToTextModels().OrderBy(m => m.Text));
            model.LooseKeywords.AddRange(entry.LooseKeywords.ToTextModels().OrderBy(m => m.Text));
            model.AllKeywords.AddRange(entry.AllKeywords.ToTextModels().OrderBy(m => m.Text));
            model.ExactKeywords.AddRange(entry.ExactKeywords.ToTextModels().OrderBy(m => m.Text));
            model.Replies.AddRange(entry.Replies.ToTextModels().OrderBy(m => m.Text));

            this.Responses.Add(model);
        }

        this.Responses.RegisterCollectionItemChanged(this.OnResponseUpdate);
    }

    private void AddResponse()
    {
        Guid id = Guid.NewGuid();
        MessageResponseModel messageResponse = new() { Id = id.ToString() };
        this.Responses.Add(messageResponse);

        this.addHandler.Handle(new AddResponse.Command(id));
    }

    private void OnResponseUpdate(object sender, PropertyChangedEventArgs e)
    {
        MessageResponseModel model = (MessageResponseModel)sender;

        UpdateResponse.Command command = new(Guid.Parse(model.Id),
                                             model.TimeoutInSeconds,
                                             model.ReplyAlways,
                                             model.Priority,
                                             model.Users.ToStrings(),
                                             model.ExactKeywords.ToStrings(),
                                             model.LooseKeywords.ToStrings(),
                                             model.AllKeywords.ToStrings(),
                                             model.Replies.ToStrings());

        this.updateHandler.Handle(command);
    }

    private void RemoveResponse(object parameter)
    {
        MessageResponseModel response = (MessageResponseModel) parameter;
        this.Responses.Remove(response);

        this.removeHandler.Handle(new RemoveResponse.Command(Guid.Parse(response.Id)));
    }

    [ExcludeFromCodeCoverage]
    public string SearchText
    {
        get => this.searchText;
        set {
            this.searchText = value;
            this.UpdateSearch(); }
    }
    
    [ExcludeFromCodeCoverage]
    private void UpdateSearch()
    {
        ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Responses);

        if (string.IsNullOrEmpty(this.SearchText))
        {
            collectionView.Filter = o => true;
        }
        else
        {
            collectionView.Filter = o =>
            {
                MessageResponseModel response = (MessageResponseModel) o;

                return response.Replies.Any(k => k.Text.Contains(this.searchText, StringComparison.OrdinalIgnoreCase))
                       || response.AllKeywords.Any(k => k.Text.Contains(this.searchText, StringComparison.OrdinalIgnoreCase))
                       || response.ExactKeywords.Any(k => k.Text.Contains(this.searchText, StringComparison.OrdinalIgnoreCase))
                       || response.LooseKeywords.Any(k => k.Text.Contains(this.searchText, StringComparison.OrdinalIgnoreCase));
            };
        }

        collectionView.Refresh();
    }
}