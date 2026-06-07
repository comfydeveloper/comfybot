using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Shared.Services;
using ComfyBot.Common.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComfyBot.Application.Scaffolding;

public class ApplicationModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IScopedServiceProvider, ScopedServiceProvider>();

        services.AddAllImplementing(typeof(ICommandHandler<>), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(ICommandHandler<,>), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(IQueryHandler<>), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(IQueryHandler<,>), ServiceLifetime.Scoped);
    }

    public void Configure(IHost _)
    {
    }
}
