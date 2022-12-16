namespace ComfyBot.Application.Responses;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Data;

using Shared;
using Shared.Contracts;
using Shared.Extensions;
using Data.Models;
using Data.Repositories;

public class ResponseTabViewModel : InitializableTab
{
    private readonly IRepository<MessageResponse> repository;
    private readonly IMapper<MessageResponse, MessageResponseModel> mapper;
    private string searchText;

    public ResponseTabViewModel(IRepository<MessageResponse> repository,
        IMapper<MessageResponse, MessageResponseModel> mapper)
    {
        this.repository = repository;
        this.mapper = mapper;

        AddResponseCommand = new DelegateCommand(AddResponse);
        RemoveResponseCommand = new ParameterCommand(RemoveResponse);
    }

    public DelegateCommand AddResponseCommand { get; }

    public ParameterCommand RemoveResponseCommand { get; }

    public ObservableCollection<MessageResponseModel> Responses { get; set; } = new();

    protected override void Initialize()
    {
        IEnumerable<MessageResponse> messageResponses = repository.GetAll().OrderBy(r => r.Priority);

        foreach (MessageResponse entity in messageResponses)
        {
            MessageResponseModel model = new MessageResponseModel();
            mapper.MapToModel(entity, model);
            Responses.Add(model);
        }

        Responses.RegisterCollectionItemChanged(OnResponseUpdate);
    }

    private void AddResponse()
    {
        MessageResponseModel messageResponse = new MessageResponseModel { Id = Guid.NewGuid().ToString() };
        Responses.Add(messageResponse);
    }

    private void RemoveResponse(object parameter)
    {
        MessageResponseModel response = (MessageResponseModel) parameter;

        Responses.Remove(response);
        repository.Remove(response.Id);
    }

    private void OnResponseUpdate(object sender, PropertyChangedEventArgs e)
    {
        MessageResponseModel model = (MessageResponseModel)sender;
        MessageResponse entity = new MessageResponse();

        mapper.MapToEntity(model, entity);
        repository.AddOrUpdate(entity);
    }

    [ExcludeFromCodeCoverage]
    public string SearchText
    {
        get => searchText;
        set { searchText = value; UpdateSearch(); }
    }

    [ExcludeFromCodeCoverage]
    private void UpdateSearch()
    {
        ICollectionView collectionView = CollectionViewSource.GetDefaultView(Responses);

        if (string.IsNullOrEmpty(SearchText))
        {
            collectionView.Filter = o => true;
        }
        else
        {
            collectionView.Filter = o =>
            {
                MessageResponseModel response = (MessageResponseModel) o;

                return response.Replies.Any(k => k.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                       || response.AllKeywords.Any(k => k.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                       || response.ExactKeywords.Any(k => k.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                       || response.LooseKeywords.Any(k => k.Text.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            };
        }

        collectionView.Refresh();
    }
}