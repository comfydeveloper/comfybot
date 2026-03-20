using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Main;
using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared.Services;
using ComfyBot.Application.Shared.Wrappers;
using ComfyBot.Application.TextCommands;
using ComfyBot.Application.Variables;
using ComfyBot.Common.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComfyBot.Application.Scaffolding;

public class ApplicationModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IMessageBox, MessageBoxWrapper>();
        services.AddSingleton<IScopedServiceProvider, ScopedServiceProvider>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<TabsViewModel>();
        services.AddTransient<ResponseTabViewModel>();
        services.AddTransient<TextCommandsTabViewModel>();
        services.AddTransient<VariablesTabViewModel>();

        services.AddAllImplementing(typeof(ICommandHandler<>), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(ICommandHandler<,>), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(IQueryHandler<>), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(IQueryHandler<,>), ServiceLifetime.Scoped);

        services.AddTransient<MainWindow>();
    }

    public void Configure(IHost _)
    {
    }
}