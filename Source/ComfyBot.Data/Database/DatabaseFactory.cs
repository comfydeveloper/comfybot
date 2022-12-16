namespace ComfyBot.Data.Database;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Wrappers;
using Settings;

using LiteDB;

[ExcludeFromCodeCoverage]
public class DatabaseFactory : IDatabaseFactory
{
    public IDatabase Create()
    {
        string databasePath = GetDatabasePath();
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath));

        return new DatabaseWrapper(new LiteDatabase(databasePath));
    }

    private static string GetDatabasePath()
    {
        return Environment.ExpandEnvironmentVariables(ApplicationSettings.Default.DatabasePath);
    }
}