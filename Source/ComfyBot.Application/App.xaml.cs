using ComfyBot.Application.Scaffolding;
using System;
using System.Collections.Generic;
using System.Windows;
using ComfyBot.Bot.ChatBot;
using ComfyBot.Bot.PubSub;
using ComfyBot.Bot.Scaffolding;
using ComfyBot.Common.Initialization;
using ComfyBot.Data.Scaffolding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using ComfyBot.Common.Scaffolding;

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

        DotNetEnv.Env.Load();

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        SetupConfiguration(builder);

        List<IModule> modules =
        [
            new ApplicationModule(),
            new DataModule(),
            new BotModule()
        ];

        RegisterModules(builder, modules);

        builder.Logging.AddSerilog();

        AppHost = builder.Build();

        ConfigureModules(AppHost, modules);

        ServiceProvider = AppHost.Services;
    }

    private static void ConfigureModules(IHost host, List<IModule> modules)
    {
        foreach (IModule projectModule in modules)
        {
            projectModule.Configure(host);
        }
    }

    private static void RegisterModules(IHostApplicationBuilder builder, List<IModule> modules)
    {
        foreach (IModule projectModule in modules)
        {
            projectModule.RegisterServices(builder.Services);
        }
    }

    private static void SetupConfiguration(IHostApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.Configure<DataSettings>(builder.Configuration.GetSection(DataSettings.SectionName));
        builder.Services.Configure<BotSettings>(builder.Configuration.GetSection(BotSettings.SectionName));
    }

    [STAThread]
    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            Log.Debug("OnStartup started.");

            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();

            IEnumerable<IInitializerJob> initializerJobs = AppHost.Services.GetServices<IInitializerJob>();

            Log.Debug("Running initialization jobs.");
            foreach (IInitializerJob job in initializerJobs)
            {
                job.Execute();
            }

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