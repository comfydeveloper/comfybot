using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ComfyBot.Application.Components;
using ComfyBot.Application.Scaffolding;
using ComfyBot.Bot.ChatBot;
using ComfyBot.Bot.PubSub;
using ComfyBot.Bot.Scaffolding;
using ComfyBot.Common.Initialization;
using ComfyBot.Data.Scaffolding;
using ComfyBot.Common.Scaffolding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace ComfyBot.Application;

public class Program
{
    public static async Task Main(String[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(LogEventLevel.Information)
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .Enrich.WithExceptionDetails()
            .CreateLogger();

        try
        {
            Log.Debug("Starting ComfyBot Application");
            DotNetEnv.Env.Load();

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            SetupConfiguration(builder);

            List<IModule> modules =
            [
                new ApplicationModule(),
                new DataModule(),
                new BotModule()
            ];

            RegisterModules(builder.Services, modules);
            builder.Logging.AddSerilog();

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            WebApplication app = builder.Build();

            List<IInitializerJob> initializerJobs = app.Services.GetServices<IInitializerJob>().ToList();
            List<IShutdownJob> shutdownJobs = app.Services.GetServices<IShutdownJob>().ToList();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            ConfigureModules(app, modules);

            Log.Debug("Running initialization jobs.");
            foreach (IInitializerJob job in initializerJobs)
            {
                job.Execute();
            }

            IComfyBot comfyBot = app.Services.GetRequiredService<IComfyBot>();
            await comfyBot.Run();

            IComfyPubSub pubSub = app.Services.GetRequiredService<IComfyPubSub>();
            pubSub.Run();

            app.Lifetime.ApplicationStopping.Register(() =>
            {
                foreach (IShutdownJob job in shutdownJobs)
                {
                    job.Complete();
                }
            });

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureModules(IHost host, List<IModule> modules)
    {
        foreach (IModule projectModule in modules)
        {
            projectModule.Configure(host);
        }
    }

    private static void RegisterModules(IServiceCollection services, List<IModule> modules)
    {
        foreach (IModule projectModule in modules)
        {
            projectModule.RegisterServices(services);
        }
    }

    private static void SetupConfiguration(WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.Configure<DataSettings>(builder.Configuration.GetSection(DataSettings.SectionName));
        builder.Services.Configure<BotSettings>(builder.Configuration.GetSection(BotSettings.SectionName));
    }
}
