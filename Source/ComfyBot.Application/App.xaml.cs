using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Exceptions;

namespace ComfyBot.Application;

using System.Windows;
using System;
using Application = System.Windows.Application;
using ComfyBot.Common.Initialization;
using System.Collections.Generic;
using ComfyBot.Bot.ChatBot;
using ComfyBot.Bot.PubSub;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
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
            .ConfigureServices(ComfyBot.Application.Startup.RegisterDependencies)
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