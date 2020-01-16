namespace ComfyBot.Application.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    using ComfyBot.Application.Shared;
    using ComfyBot.Application.Shared.Extensions;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    public class ResponseTabViewModel : InitializableTab
    {
        private readonly IRepository<MessageResponse> repository;

        public ResponseTabViewModel(IRepository<MessageResponse> repository)
        {
            this.repository = repository;

            this.AddResponseCommand = new DelegateCommand(AddResponse);
        }

        public DelegateCommand AddResponseCommand { get; }

        public ObservableCollection<ResponseModel> Responses { get; set; } = new ObservableCollection<ResponseModel>();

        protected override void Initialize()
        {
            IEnumerable<MessageResponse> messageResponses = this.repository.GetAll();

            IEnumerable<ResponseModel> models = messageResponses.Select(MapToModel);
            this.Responses.AddRange(models);

            this.Responses.RegisterCollectionItemChanged(this.OnResponseUpdate);
        }

        private ResponseModel MapToModel(MessageResponse response)
        {
            ResponseModel model = new ResponseModel
                                       {
                                           Id = response.Id,
                                           Timeout = response.TimeoutInSeconds
                                       };

            model.Users.AddRange(response.Users.ToTextModels());
            model.AllKeywords.AddRange(response.AllKeywords.ToTextModels());
            model.LooseKeywords.AddRange(response.LooseKeywords.ToTextModels());
            model.ExactKeywords.AddRange(response.ExactKeywords.ToTextModels());
            model.Replies.AddRange(response.Replies.ToTextModels());

            return model;
        }

        private void AddResponse()
        {
            ResponseModel response = new ResponseModel { Id = Guid.NewGuid().ToString() };
            this.Responses.Add(response);
        }

        private void OnResponseUpdate(object sender, PropertyChangedEventArgs e)
        {
            ResponseModel response = (ResponseModel)sender;
            this.repository.AddOrUpdate(ToResponse(response));
        }

        private MessageResponse ToResponse(ResponseModel model)
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