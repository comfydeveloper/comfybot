using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ComfyBot.Bot.Tests.TestHelpers;

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> enumerator;

    public TestAsyncEnumerator(IEnumerator<T> enumerator)
    {
        this.enumerator = enumerator;
    }

    public T Current => this.enumerator.Current;

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(this.enumerator.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        this.enumerator.Dispose();
        return default;
    }
}

internal class TestAsyncQueryProvider<TEntity> : IQueryProvider
{
    private readonly IQueryProvider inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        this.inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return this.inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return this.inner.Execute<TResult>(expression);
    }
}
