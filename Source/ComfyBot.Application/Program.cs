namespace ComfyBot.Application
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using ComfyBot.Bot.ChatBot;
    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Data.Database;

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

            MainWindow mainWindow = serviceProvider.GetService<MainWindow>();
            IComfyBot comfyBot = serviceProvider.GetService<IComfyBot>();
            comfyBot.Run();

            App app = new App();
            app.Run(mainWindow);
        }

        private static void RegisterServices(IServiceCollection collection)
        {
            Assembly[] assemblies = {
                                        typeof(Program).Assembly,
                                        typeof(ICommandHandler).Assembly,
                                        typeof(IDatabaseFactory).Assembly
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
}