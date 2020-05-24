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
            model.Timeout = entity.TimeoutInSeconds;
            model.Replies.AddRange(entity.Replies.ToTextModels().OrderBy(m => m.Text));
            model.Commands.AddRange(entity.Commands.ToTextModels().OrderBy(m => m.Text));
        }

        public void MapToEntity(TextCommandModel model, TextCommand entity)
        {
            entity.Id = model.Id;
            entity.TimeoutInSeconds = model.Timeout;
            entity.Replies = model.Replies.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => r.Text).ToList();
            entity.Commands = model.Commands.Where(r => !string.IsNullOrEmpty(r.Text)).Select(r => r.Text).ToList();
        }
    }
}