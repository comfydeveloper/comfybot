namespace ComfyBot.Application.Data
{
    using ComfyBot.Application.Data.Models;

    public interface IRepository<T> where T: Entity
    {
        T Get(string key);

        void AddOrUpdate(T model);
    }
}