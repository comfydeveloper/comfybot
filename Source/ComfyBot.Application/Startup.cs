using System;
using System.Linq;
using System.Reflection;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Common.Http;
using ComfyBot.Data.Database;
using Microsoft.Extensions.DependencyInjection;

namespace ComfyBot.Application;

// TODO [Shae] Remove this 
[Obsolete]
public class Startup
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        Assembly[] assemblies =
        [
            typeof(ICommandHandler).Assembly,
            typeof(IDatabaseFactory).Assembly,
            typeof(IHttpService).Assembly
        ];

        foreach (Assembly assembly in assemblies)
        {
            RegisterImplementations(services, assembly);
            RegisterImplementationsWithoutInterfaces(services, assembly);
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

    public static void Initialize()
    {
        AssertDatabaseDirectoryExists();
    }

    private static void AssertDatabaseDirectoryExists()
    {
        // TODO [Shae] Remove/Assure this in another place
        //var databasePath = EnvironmentExtensions.GetDatabasePath();
        //Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
    }
}