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
            model.Priority = entity.Priority;
            model.Users.AddRange(entity.Users.ToTextModels().OrderBy(m => m.Text));
            model.LooseKeywords.AddRange(entity.LooseKeywords.ToTextModels().OrderBy(m => m.Text));
            model.AllKeywords.AddRange(entity.AllKeywords.ToTextModels().OrderBy(m => m.Text));
            model.ExactKeywords.AddRange(entity.ExactKeywords.ToTextModels().OrderBy(m => m.Text));
            model.Replies.AddRange(entity.Replies.ToTextModels().OrderBy(m => m.Text));
        }

        public void MapToEntity(MessageResponseModel model, MessageResponse entity)
        {
            entity.Id = model.Id;
            entity.TimeoutInSeconds = model.Timeout;
            entity.Priority = model.Priority;
            entity.Users = model.Users.Where(u => !string.IsNullOrEmpty(u.Text)).Select(u => u.Text).ToList();
            entity.LooseKeywords = model.LooseKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList();
            entity.AllKeywords = model.AllKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList();
            entity.ExactKeywords = model.ExactKeywords.Where(k => !string.IsNullOrEmpty(k.Text)).Select(k => k.Text).ToList();
            entity.Replies = model.Replies.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => r.Text).ToList();
        }
    }
}