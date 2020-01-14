namespace ComfyBot.Data.Repositories
{
    using ComfyBot.Data.Models;

    public interface IRepository<T> where T : Entity
    {
        T Get(string key);

        void AddOrUpdate(T model);
    }
}