using ComfyBot.Data.Database;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Data.Repositories;

[ExcludeFromCodeCoverage]
public class QueryableRepository : IQueryableRepository
{
    private readonly DataContext context;

    public QueryableRepository(DataContext context)
    {
        this.context = context;
    }

    public IQueryable<T> Query<T>() where T : class
    {
        return this.context.Set<T>();
    }

    public void Add<T>(T entity) where T : class
    {
        this.context.Add(entity);
    }

    public void Remove<T>(T entity) where T : class
    {
        this.context.Remove(entity);
    }

    public async Task SaveChanges()
    {
        await this.context.SaveChangesAsync();
    }
}