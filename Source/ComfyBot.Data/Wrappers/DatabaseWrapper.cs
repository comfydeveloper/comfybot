using System;
using System.Diagnostics.CodeAnalysis;
using LiteDB;

namespace ComfyBot.Data.Wrappers;

[ExcludeFromCodeCoverage]
public class DatabaseWrapper : IDatabase
{
    private readonly ILiteDatabase liteDatabase;

    public DatabaseWrapper(ILiteDatabase liteDatabase)
    {
        this.liteDatabase = liteDatabase;
    }

    public ILiteCollection<T> GetCollection<T>(string name)
    {
        return this.liteDatabase.GetCollection<T>(name);
    }

    public void Dispose()
    {
        try
        {
            this.liteDatabase?.Dispose();
        }
        catch(ObjectDisposedException disposedException)
        {
            Console.WriteLine($"Failed to dispose database {disposedException.Message} - {disposedException.StackTrace}");
        }
    }
}