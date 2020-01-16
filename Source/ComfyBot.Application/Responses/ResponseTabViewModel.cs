namespace ComfyBot.Application.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection.Metadata;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.Shared.Extensions;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    public class ResponseTabViewModel : InitializableTab
    {
        private readonly IRepository<MessageResponse> repository;
        private readonly IMapper<MessageResponse, MessageResponseModel> mapper;

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
            IEnumerable<MessageResponse> messageResponses = this.repository.GetAll();

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
            MessageResponseModel messageResponse = (MessageResponseModel)sender;
            this.repository.AddOrUpdate(ToResponse(messageResponse));
        }

        private MessageResponse ToResponse(MessageResponseModel model)
        {
            MessageResponse response = new MessageResponse
                                       {
                                           Id = model.Id,
                                           TimeoutInSeconds = model.Timeout,
                                           Users = model.Users.Select(k => k.Text).ToList(),
                                           AllKeywords = model.AllKeywords.Select(k => k.Text).ToList(),
                                           LooseKeywords = model.LooseKeywords.Select(k => k.Text).ToList(),
                                           ExactKeywords = model.ExactKeywords.Select(k => k.Text).ToList(),
                                           Replies = model.Replies.Select(k => k.Text).ToList(),
                                       };
            return response;
        }
    }
}