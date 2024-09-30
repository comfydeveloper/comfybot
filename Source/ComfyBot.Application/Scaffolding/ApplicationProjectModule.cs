using ComfyBot.Application.Configuration;
using ComfyBot.Application.Main;
using ComfyBot.Application.Responses;
using ComfyBot.Application.Shared.Contracts;
using ComfyBot.Application.Shared.Wrappers;
using ComfyBot.Application.TextCommands;
using ComfyBot.Common.Scaffolding;
using ComfyBot.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace ComfyBot.Application.Scaffolding;

public class ApplicationProjectModule : IProjectModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IMapper<MessageResponseOld, MessageResponseModel>, MessageResponseMapper>();
        services.AddTransient<IMapper<TextCommandOld, TextCommandModel>, TextCommandMapper>();
        services.AddTransient<IMessageBox, MessageBoxWrapper>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<TabsViewModel>();
        services.AddTransient<ResponseTabViewModel>();
        services.AddTransient<ConfigurationTabViewModel>();
        services.AddTransient<TextCommandsTabViewModel>();

        services.AddTransient<MainWindow>();
    }

    public void Configure(IHost applicationBuilder)
    {
    }
}