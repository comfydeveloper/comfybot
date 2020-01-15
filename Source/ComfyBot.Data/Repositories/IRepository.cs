namespace ComfyBot.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using ComfyBot.Data.Models;

    public interface IRepository<T> where T : Entity
    {
        T Get(Expression<Func<T, bool>> predicate);

        void AddOrUpdate(T model);

        void Remove(string id);

        IEnumerable<T> GetAll();
    }
}