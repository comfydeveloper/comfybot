using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Common.Http;
using ComfyBot.Common.Scaffolding;
using ComfyBot.Data.Database;
using ComfyBot.Data.Scaffolding;
using ComfyBot.Settings.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ComfyBot.Application;

public class Startup
{
    public static void RegisterDependencies(IServiceCollection services)
    {
        Assembly[] assemblies =
        [
            typeof(Startup).Assembly,
            typeof(ICommandHandler).Assembly,
            typeof(IDatabaseFactory).Assembly,
            typeof(IHttpService).Assembly
        ];

        List<IProjectModule> modules =
        [
            new DataProjectModule()
        ];

        foreach (IProjectModule projectModule in modules)
        {
            projectModule.RegisterServices(services);
        }

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
        var databasePath = EnvironmentExtensions.GetDatabasePath();
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
    }
}