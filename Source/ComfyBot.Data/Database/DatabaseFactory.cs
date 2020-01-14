namespace ComfyBot.Data.Database
{
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Data.Wrappers;
    using ComfyBot.Settings;

    using LiteDB;

    [ExcludeFromCodeCoverage]
    public class DatabaseFactory : IDatabaseFactory
    {
        public IDatabase Create()
        {
            return new DatabaseWrapper(new LiteDatabase(ApplicationSettings.Default.DatabasePath));
        }
    }
}