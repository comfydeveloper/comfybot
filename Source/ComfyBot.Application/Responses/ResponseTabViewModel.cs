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
    private readonly IQueryableRepository repository;
    private readonly IMapper<MessageResponse, MessageResponseModel> mapper;
    private string searchText;

    public ResponseTabViewModel(IQueryableRepository repository,
        IMapper<MessageResponse, MessageResponseModel> mapper)
    {
        this.repository = repository;
        this.mapper = mapper;

        this.AddResponseCommand = new DelegateCommand(this.AddResponse);
        this.RemoveResponseCommand = new ParameterCommand(this.RemoveResponse);
    }

    public DelegateCommand AddResponseCommand { get; }

    public ParameterCommand RemoveResponseCommand { get; }

    public ObservableCollection<MessageResponseModel> Responses { get; set; } = [];

    protected override void Initialize()
    {
        IEnumerable<MessageResponse> messageResponses = this.repository.Query<MessageResponse>().OrderBy(r => r.Priority).ToList();

        foreach (MessageResponse entity in messageResponses)
        {
            MessageResponseModel model = new();
            this.mapper.MapToModel(entity, model);
            this.Responses.Add(model);
        }

        this.Responses.RegisterCollectionItemChanged(this.OnResponseUpdate);
    }

    private void AddResponse()
    {
        MessageResponseModel messageResponse = new() { Id = Guid.NewGuid().ToString() };
        this.Responses.Add(messageResponse);
    }

    private void RemoveResponse(object parameter)
    {
        MessageResponseModel response = (MessageResponseModel) parameter;

        this.Responses.Remove(response);
        this.repository.Remove(response.Id);
    }

    private void OnResponseUpdate(object sender, PropertyChangedEventArgs e)
    {
        MessageResponseModel model = (MessageResponseModel)sender;

        MessageResponse message = this.repository.Query<MessageResponse>().FirstOrDefault(x => x.Id == Guid.Parse(model.Id));

        if (message == null)
        {
            MessageResponse newResponse = new()
            {
                Users = model.Users.Where(u => !string.IsNullOrEmpty(u.Text)).Select(u => u.Text).ToList(),
                LooseKeywords = model.LooseKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList(),
                AllKeywords = model.AllKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList(),
                ExactKeywords = model.ExactKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList(),
                Replies = model.Replies.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => r.Text).ToList(),
                LastUsedAt = null,
                TimeoutInSeconds = 30,
                UseCount = 0,
                Priority = model.Priority,
                AlwaysReply = model.ReplyAlways,
                Id = Guid.Parse(model.Id),
                CreatedAt = DateTime.Now
            };
            
            this.repository.Add(newResponse);
        }
        else
        {
            this.mapper.MapToEntity(model, message);
        }

        this.repository.SaveChanges();
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