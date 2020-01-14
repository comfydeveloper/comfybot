namespace ComfyBot.Data.Wrappers
{
    using System;
    using System.Linq.Expressions;

    public interface ILiteCollection<T>
    {
        T FindOne(Expression<Func<T, bool>> predicate);

        void Update(T entity);

        void Insert(T entity);
    }
}