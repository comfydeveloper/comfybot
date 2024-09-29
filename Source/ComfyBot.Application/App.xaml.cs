using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using ComfyBot.Bot.ChatBot;
using ComfyBot.Bot.PubSub;
using ComfyBot.Common.Initialization;
using ComfyBot.Data.Repositories;
using ComfyBot.Data.Scaffolding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace ComfyBot.Application;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static IHost AppHost { get; private set; }

    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(LogEventLevel.Information)
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .Enrich.WithExceptionDetails()
            .CreateLogger();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        Application.Startup.RegisterDependencies(builder.Services);
        Application.Startup.Initialize();

        builder.Logging.AddSerilog();

        SetupConfiguration(builder);

        AppHost = builder.Build();
        ServiceProvider = AppHost.Services;
    }

    private static void SetupConfiguration(IHostApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.Configure<DataSettings>(builder.Configuration.GetSection(DataSettings.SectionName));
    }

    [STAThread]
    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            Log.Debug("OnStartup started.");

            Application.Startup.Initialize();
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();

            IEnumerable<IInitializerJob> initializerJobs = AppHost.Services.GetServices<IInitializerJob>();

            Log.Debug("Running initialization jobs.");
            foreach (IInitializerJob job in initializerJobs)
            {
                job.Execute();
            }

            var a = AppHost.Services.GetRequiredService<DataSettings>();
            var b = AppHost.Services.GetRequiredService<IQueryableRepository>();

            IComfyBot comfyBot = AppHost.Services.GetService<IComfyBot>();
            comfyBot.Run();
            IComfyPubSub service = AppHost.Services.GetService<IComfyPubSub>();
            service.Run();

            startupForm.Show();

            base.OnStartup(e);
            Log.Debug("OnStartup finished.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Failed on startup");
        }
        
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        IEnumerable<IShutdownJob> completableTask = AppHost!.Services.GetServices<IShutdownJob>();

        foreach (IShutdownJob job in completableTask)
        {
            job.Complete();
        }

        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}