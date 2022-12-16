namespace ComfyBot.Data.Wrappers
{
    using System.Diagnostics.CodeAnalysis;

    using LiteDB;

    [ExcludeFromCodeCoverage]
    public class DatabaseWrapper : IDatabase
    {
        private readonly LiteDatabase liteDatabase;

        public DatabaseWrapper(LiteDatabase liteDatabase)
        {
            this.liteDatabase = liteDatabase;
        }

        public ILiteCollection<T> GetCollection<T>(string name)
        {
            return liteDatabase.GetCollection<T>(name);
        }

        public void Dispose()
        {
            liteDatabase?.Dispose();
        }
    }
}