namespace ComfyBot.Application.Data
{
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Application.Data.Wrappers;

    using LiteDB;

    using Microsoft.Extensions.Configuration;

    [ExcludeFromCodeCoverage]
    public class DatabaseFactory : IDatabaseFactory
    {
        private readonly IConfiguration configuration;

        public DatabaseFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IDatabase Create()
        {
            return new DatabaseWrapper(new LiteDatabase(this.configuration["databasePath"]));
        }
    }
}