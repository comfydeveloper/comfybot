namespace ComfyBot.Application.Shoutouts
{
    using ComfyBot.Application.Shared.Contracts;
    using ComfyBot.Data.Models;

    public class ShoutoutMapper : IMapper<Shoutout, ShoutoutModel>
    {
        public void MapToModel(Shoutout entity, ShoutoutModel model)
        {
            model.Id = entity.Id;
            model.Command = entity.Command;
            model.Message = entity.Message;
        }

        public void MapToEntity(ShoutoutModel model, Shoutout entity)
        {
            entity.Id = model.Id;
            entity.Command = model.Command;
            entity.Message = model.Message;
        }
    }
}