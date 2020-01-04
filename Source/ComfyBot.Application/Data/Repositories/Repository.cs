namespace ComfyBot.Application.Data.Repositories
{
    using ComfyBot.Application.Data.Models;
    using ComfyBot.Application.Data.Wrappers;

    public abstract class Repository<T> : IRepository<T>
        where T : Entity
    {
        private readonly IDatabaseFactory databaseFactory;
        private readonly string table;

        protected Repository(IDatabaseFactory databaseFactory, string table)
        {
            this.databaseFactory = databaseFactory;
            this.table = table;
        }

        public T Get(string key)
        {
            using (IDatabase database = this.databaseFactory.Create())
            {
                ILiteCollection<T> collection = database.GetCollection<T>(table);
                T model = collection.FindOne(x => x.Id == key);

                return model;
            }
        }

        public void AddOrUpdate(T model)
        {
            using (IDatabase database = this.databaseFactory.Create())
            {
                ILiteCollection<T> collection = database.GetCollection<T>(this.table);
                T entity = collection.FindOne(x => x.Id == model.Id);

                if (entity == null)
                {
                    collection.Insert(model);
                }
                else
                {
                    Update(model, entity);
                    collection.Update(entity);
                }
            }
        }

        protected abstract void Update(T source, T target);
    }
}