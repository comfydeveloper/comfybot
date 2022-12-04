namespace ComfyBot.Application.Responses
{
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

            this.AddResponseCommand = new DelegateCommand(this.AddResponse);
            this.RemoveResponseCommand = new ParameterCommand(this.RemoveResponse);
        }

        public DelegateCommand AddResponseCommand { get; }

        public ParameterCommand RemoveResponseCommand { get; }

        public ObservableCollection<MessageResponseModel> Responses { get; set; } = new ObservableCollection<MessageResponseModel>();

        protected override void Initialize()
        {
            IEnumerable<MessageResponse> messageResponses = this.repository.GetAll().OrderBy(r => r.Priority);

            foreach (MessageResponse entity in messageResponses)
            {
                MessageResponseModel model = new MessageResponseModel();
                this.mapper.MapToModel(entity, model);
                this.Responses.Add(model);
            }

            this.Responses.RegisterCollectionItemChanged(this.OnResponseUpdate);
        }

        private void AddResponse()
        {
            MessageResponseModel messageResponse = new MessageResponseModel { Id = Guid.NewGuid().ToString() };
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
            MessageResponse entity = new MessageResponse();

            this.mapper.MapToEntity(model, entity);
            this.repository.AddOrUpdate(entity);
        }

        [ExcludeFromCodeCoverage]
        public string SearchText
        {
            get => this.searchText;
            set { this.searchText = value; this.UpdateSearch(); }
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
}