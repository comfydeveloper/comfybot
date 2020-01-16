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
            entity.Users = model.Users.Select(u => u.Text).ToList();
            entity.LooseKeywords = model.LooseKeywords.Select(u => u.Text).ToList();
            entity.AllKeywords = model.AllKeywords.Select(u => u.Text).ToList();
            entity.ExactKeywords = model.ExactKeywords.Select(u => u.Text).ToList();
            entity.Replies = model.Replies.Select(u => u.Text).ToList();
        }
    }
}