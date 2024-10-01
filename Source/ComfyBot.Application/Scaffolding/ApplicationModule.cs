using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Main;
using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared.Wrappers;
using ComfyBot.Application.TextCommands;
using ComfyBot.Common.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComfyBot.Application.Scaffolding;

public class ApplicationModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IMessageBox, MessageBoxWrapper>();

        services.AddAutoMapper(this.GetType().Assembly);

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<TabsViewModel>();
        services.AddTransient<ResponseTabViewModel>();
        services.AddTransient<TextCommandsTabViewModel>();

        services.AddAllImplementing(typeof(ICommandHandler<>), ServiceLifetime.Transient);
        services.AddAllImplementing(typeof(ICommandHandler<,>), ServiceLifetime.Transient);
        services.AddAllImplementing(typeof(IQueryHandler<>), ServiceLifetime.Transient);
        services.AddAllImplementing(typeof(IQueryHandler<,>), ServiceLifetime.Transient);

        services.AddTransient<MainWindow>();
    }

    public void Configure(IHost _)
    {
    }
}