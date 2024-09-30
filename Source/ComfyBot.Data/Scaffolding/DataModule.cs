using ComfyBot.Common.Scaffolding;
using ComfyBot.Data.Database;
using ComfyBot.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComfyBot.Data.Scaffolding;

public class DataModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddDbContext<DataContext>(contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton);
        services.AddTransient<IQueryableRepository, QueryableRepository>();
    }

    public void Configure(IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        using DataContext context = scope.ServiceProvider.GetService<DataContext>();
        context.Database.Migrate();
    }
}