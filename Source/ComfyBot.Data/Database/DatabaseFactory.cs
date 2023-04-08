using System.Diagnostics.CodeAnalysis;
using ComfyBot.Data.Wrappers;
using ComfyBot.Settings.Extensions;
using LiteDB;

namespace ComfyBot.Data.Database;

[ExcludeFromCodeCoverage]
public class DatabaseFactory : IDatabaseFactory
{
    public IDatabase Create()
    {
        string databasePath = EnvironmentExtensions.GetDatabasePath();
        
        return new DatabaseWrapper(new LiteDatabase($"Filename={databasePath}; Connection=Shared;"));
    }
}