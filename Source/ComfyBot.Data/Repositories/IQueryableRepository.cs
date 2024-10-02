using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Data.Repositories;

public interface IQueryableRepository
{
    public IQueryable<T> Query<T>() where T : class;

    public void Add<T>(T entity) where T : class;

    public void Remove<T>(T entity) where T : class;

    public Task SaveChanges();
}