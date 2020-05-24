namespace ComfyBot.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using ComfyBot.Data.Database;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Wrappers;

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

        public T Get(Expression<Func<T, bool>> predicate)
        {
            using IDatabase database = this.databaseFactory.Create();

            ILiteCollection<T> collection = database.GetCollection<T>(this.table);
            T model = collection.FindOne(predicate);

            return model;
        }

        public void AddOrUpdate(T model)
        {
            using IDatabase database = this.databaseFactory.Create();

            ILiteCollection<T> collection = database.GetCollection<T>(this.table);
            T entity = collection.FindOne(x => x.Id == model.Id);

            if (entity == null)
            {
                model.DateOfCreation = DateTime.Now;
                collection.Insert(model);
            }
            else
            {
                this.Update(model, entity);
                collection.Update(entity);
            }
        }

        public void Remove(string id)
        {
            using IDatabase database = this.databaseFactory.Create();

            ILiteCollection<T> collection = database.GetCollection<T>(this.table);
            collection.Remove(t => t.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            using IDatabase database = this.databaseFactory.Create();

            ILiteCollection<T> collection = database.GetCollection<T>(this.table);
            return collection.FindAll();
        }

        protected abstract void Update(T source, T target);
    }
}