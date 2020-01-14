namespace ComfyBot.Data.Wrappers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    using LiteDB;

    [ExcludeFromCodeCoverage]
    public class LiteCollectionWrapper<T> : ILiteCollection<T>
    {
        private readonly LiteCollection<T> collection;

        public LiteCollectionWrapper(LiteCollection<T> collection)
        {
            this.collection = collection;
        }

        public T FindOne(Expression<Func<T, bool>> predicate)
        {
            return this.collection.FindOne(predicate);
        }

        public void Update(T entity)
        {
            this.collection.Update(entity);
        }

        public void Insert(T entity)
        {
            this.collection.Insert(entity);
        }
    }
}