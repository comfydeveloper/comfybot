using System;
using System.Collections.Generic;
using System.Windows;
using ComfyBot.Bot.ChatBot;
using ComfyBot.Bot.PubSub;
using ComfyBot.Common.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace ComfyBot.Application;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public static IHost? AppHost { get; private set; }

    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(LogEventLevel.Information)
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .Enrich.WithExceptionDetails()
            .CreateLogger();

        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices(Application.Startup.RegisterDependencies)
            .UseSerilog()
            .Build();

        ServiceProvider = AppHost.Services;
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
        IEnumerable<ICompletableJob> completableTask = AppHost!.Services.GetServices<ICompletableJob>();

        foreach (ICompletableJob job in completableTask)
        {
            job.Complete();
        }

        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}