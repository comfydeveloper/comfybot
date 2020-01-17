namespace ComfyBot.Application.TextCommands
{
    using System.Linq;

    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Application.Shared.Extensions;
    using ComfyBot.Data.Models;

    public class TextCommandMapper : IMapper<TextCommand, TextCommandModel>
    {
        public void MapToModel(TextCommand entity, TextCommandModel model)
        {
            model.Id = entity.Id;
            model.Command = entity.Command;
            model.Timeout = entity.TimeoutInSeconds;
            model.Replies.AddRange(entity.Replies.ToTextModels());
        }

        public void MapToEntity(TextCommandModel model, TextCommand entity)
        {
            entity.Id = model.Id;
            entity.Command = model.Command;
            entity.TimeoutInSeconds = model.Timeout;
            entity.Replies = model.Replies.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => r.Text).ToList();
        }
    }
}