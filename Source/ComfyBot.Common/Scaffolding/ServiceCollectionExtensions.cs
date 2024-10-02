using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace ComfyBot.Common.Scaffolding;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddAllImplementing(this IServiceCollection services, Type serviceType, ServiceLifetime lifetime)
    {
        AddAllImplementing(services, serviceType, lifetime, Assembly.GetCallingAssembly());
    }

    private static void AddAllImplementing(this IServiceCollection services, Type serviceType, ServiceLifetime lifetime, params Assembly[] assemblies)
    {
        assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => !t.IsAbstract)
            .Where(t => !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Select(i => new
                {
                    Interface = i,
                    Type = t
                })
            )
            .Where(x => GetTypeDefinition(x.Interface) == serviceType)
            .ToList()
            .ForEach(x => services.Add(new ServiceDescriptor(x.Interface, x.Type, lifetime)));
    }

    private static Type GetTypeDefinition(Type type)
    {
        return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
    }
}