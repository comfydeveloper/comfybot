using ComfyBot.Common.Scaffolding;
using ComfyBot.Data.Database;
using ComfyBot.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ComfyBot.Data.Scaffolding;

public class DataProjectModule : IProjectModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddDbContext<DataContext>();
        services.AddTransient<IQueryableRepository, QueryableRepository>();
    }
}