using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ComfyBot.Data.Models;

namespace ComfyBot.Data.Repositories;

public interface IRepository<T> where T : Entity
{
    T Get(Expression<Func<T, bool>> predicate);

    void Write(T model);

    void Remove(string id);

    IEnumerable<T> GetAll();
}