namespace ComfyBot.Application
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Application.Bot;
    using ComfyBot.Application.Bot.Commands;
    using ComfyBot.Application.Bot.Initialization;
    using ComfyBot.Application.Data;
    using ComfyBot.Application.Data.Models;
    using ComfyBot.Application.Data.Repositories;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = SetupConfiguration();

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.Add(new ServiceDescriptor(typeof(IConfiguration), provider => configuration, ServiceLifetime.Singleton));
            RegisterServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IComfyBot comfyBot = serviceProvider.GetService<IComfyBot>();
            comfyBot.Run();

            Console.ReadLine();
        }

        private static void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<IComfyBot, ComfyBot>();
            collection.AddTransient<ITwitchClientFactory, TwitchClientFactory>();
            collection.AddTransient<ICommandHandler, TestCommandHandler>();
            collection.AddTransient<ICommandHandler, ShoutoutCommandHandler>();
            collection.AddTransient<IDatabaseFactory, DatabaseFactory>();
            collection.AddTransient<IRepository<Shoutout>, ShoutoutRepository>();
        }

        private static IConfiguration SetupConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                                           .AddJsonFile("appsettings.json", true, true)
                                           .Build();

            return configuration;
        }
    }
}