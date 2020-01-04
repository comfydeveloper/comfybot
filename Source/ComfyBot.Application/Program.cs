namespace ComfyBot.Application
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = SetupConfiguration();

            IServiceCollection services = new ServiceCollection();
            services.Add(new ServiceDescriptor(typeof(IConfiguration), provider => configuration, ServiceLifetime.Singleton));



            IServiceProvider serviceProvider = services.BuildServiceProvider();

            Console.ReadLine();
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