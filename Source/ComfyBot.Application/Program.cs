namespace ComfyBot.Application
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Bot.ChatBot;
    using ComfyBot.Bot.ChatBot.Commands;
    using ComfyBot.Bot.Initialization;
    using ComfyBot.Data.Database;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            RegisterServices(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            MainWindow mainWindow = serviceProvider.GetService<MainWindow>();
            IComfyBot comfyBot = serviceProvider.GetService<IComfyBot>();
            comfyBot.Run();

            App app = new App();
            app.Run(mainWindow);


            Console.ReadLine();
        }

        private static void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<IComfyBot, ComfyBot>();
            collection.AddTransient<ITwitchClientFactory, TwitchClientFactory>();
            collection.AddTransient<MainWindow>();
            collection.AddTransient<ICommandHandler, ShoutoutCommandHandler>();
            collection.AddTransient<IDatabaseFactory, DatabaseFactory>();
            collection.AddTransient<IRepository<Shoutout>, ShoutoutRepository>();
        }
    }
}