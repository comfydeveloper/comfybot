namespace ComfyBot.Application.Shared.Contracts
{
    public interface IMapper<TEntity, TModel>
    {
        void MapToModel(TEntity entity, TModel model);

        void MapToEntity(TModel model, TEntity entity);
    }
}