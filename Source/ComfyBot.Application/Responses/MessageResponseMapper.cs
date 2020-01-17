namespace ComfyBot.Application.Responses
{
    using System.Linq;

    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.Shared.Extensions;
    using ComfyBot.Data.Models;

    public class MessageResponseMapper : IMapper<MessageResponse, MessageResponseModel>
    {
        public void MapToModel(MessageResponse entity, MessageResponseModel model)
        {
            model.Id = entity.Id;
            model.Timeout = entity.TimeoutInSeconds;
            model.Users.AddRange(entity.Users.ToTextModels());
            model.LooseKeywords.AddRange(entity.LooseKeywords.ToTextModels());
            model.AllKeywords.AddRange(entity.AllKeywords.ToTextModels());
            model.ExactKeywords.AddRange(entity.ExactKeywords.ToTextModels());
            model.Replies.AddRange(entity.Replies.ToTextModels());
        }

        public void MapToEntity(MessageResponseModel model, MessageResponse entity)
        {
            entity.Id = model.Id;
            entity.TimeoutInSeconds = model.Timeout;
            entity.Users = model.Users.Where(u => !string.IsNullOrEmpty(u.Text)).Select(u => u.Text).ToList();
            entity.LooseKeywords = model.LooseKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList();
            entity.AllKeywords = model.AllKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList();
            entity.ExactKeywords = model.ExactKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList();
            entity.Replies = model.Replies.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => r.Text).ToList();
        }
    }
}