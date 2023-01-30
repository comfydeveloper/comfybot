using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ComfyBot.Application.Shared.Contracts;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Common.Http;
using ComfyBot.Data.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComfyBot.Application;

public class Startup
{
    public static void RegisterDependencies(IServiceCollection collection)
    {
        var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true);
        var configuration = configBuilder.Build();
        var appSettings = new AppSettings();
        configuration.Bind(appSettings);
        collection.AddSingleton(appSettings);

        Assembly[] assemblies = {
            typeof(Startup).Assembly,
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
            .Where(type => type.Namespace!.StartsWith("ComfyBot")
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
            select new { service, implementation = type };

        foreach (var registration in registrations)
        {
            collection.AddTransient(registration.service, registration.implementation);
        }
    }
}