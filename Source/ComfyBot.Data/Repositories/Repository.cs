using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ComfyBot.Data.Database;
using ComfyBot.Data.Models;
using ComfyBot.Data.Wrappers;
using LiteDB;

namespace ComfyBot.Data.Repositories;

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
        using IDatabase database = databaseFactory.Create();

        ILiteCollection<T> collection = database.GetCollection<T>(table);
        T model = collection.FindOne(predicate);

        return model;
    }

    public void Write(T model)
    {
        using IDatabase database = databaseFactory.Create();

        ILiteCollection<T> collection = database.GetCollection<T>(table);
        T entity = collection.FindOne(x => x.Id == model.Id);

        if (entity == null)
        {
            model.DateOfCreation = DateTime.Now;
            collection.Insert(model);
        }
        else
        {
            Update(model, entity);
            collection.Update(entity);
        }
    }

    public void Remove(string id)
    {
        using IDatabase database = databaseFactory.Create();

        ILiteCollection<T> collection = database.GetCollection<T>(table);
        collection.DeleteMany(t => t.Id == id);
    }

    public IEnumerable<T> GetAll()
    {
        using IDatabase database = databaseFactory.Create();

        ILiteCollection<T> collection = database.GetCollection<T>(table);
        return collection.FindAll().ToList();
    }

    protected abstract void Update(T source, T target);
}