namespace ComfyBot.Application;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using Bot.ChatBot;
using Bot.ChatBot.Commands;
using Bot.PubSub;
using Common.Http;
using Common.Initialization;
using Data.Database;

using Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public class Program
{
    [STAThread]
    public static void Main()
    {
        IServiceCollection serviceCollection = new ServiceCollection();

        RegisterServices(serviceCollection);

        IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

        IEnumerable<IInitializerJob> initializerJobs = serviceProvider.GetServices<IInitializerJob>();

        foreach (IInitializerJob job in initializerJobs)
        {
            job.Execute();
        }

        MainWindow mainWindow = serviceProvider.GetService<MainWindow>();
        IComfyBot comfyBot = serviceProvider.GetService<IComfyBot>();
        comfyBot.Run();
        IComfyPubSub service = serviceProvider.GetService<IComfyPubSub>();
        service.Run();

        App app = new App();
        app.Run(mainWindow);


        IEnumerable<ICompletableJob> completableTask = serviceProvider.GetServices<ICompletableJob>();

        foreach (ICompletableJob job in completableTask)
        {
            job.Complete();
        }
    }

    private static void RegisterServices(IServiceCollection collection)
    {
        Assembly[] assemblies = {
            typeof(Program).Assembly,
            typeof(ICommandHandler).Assembly,
            typeof(IDatabaseFactory).Assembly,
            typeof(IHttpService).Assembly
        };

        foreach (Assembly assembly in assemblies)
        {
            RegisterImplementations(collection, assembly);
            RegisterImplementationsWithoutInterfaces(collection, assembly);
        }
    }

    private static void RegisterImplementationsWithoutInterfaces(IServiceCollection collection, Assembly assembly)
    {
        Type[] registrations = assembly.GetExportedTypes()
            .Where(type => type.Namespace.StartsWith("ComfyBot")
                           && !type.IsAbstract
                           && !type.GetInterfaces().Any())
            .ToArray();

        foreach (var registration in registrations)
        {
            collection.AddTransient(registration);
        }

        collection.AddTransient(typeof(MainWindow));
    }

    private static void RegisterImplementations(IServiceCollection collection, Assembly assembly)
    {
        var registrations = from type in assembly.GetExportedTypes()
            where type.Namespace.StartsWith("ComfyBot")
                  && !type.IsAbstract
                  && (!type.Name.Contains("Wrapper") || type.GetConstructor(Type.EmptyTypes) != null)
            from service in type.GetInterfaces()
            select new {service, implementation = type};

        foreach (var registration in registrations)
        {
            collection.AddTransient(registration.service, registration.implementation);
        }
    }
}