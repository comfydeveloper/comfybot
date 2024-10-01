using System.Diagnostics.CodeAnalysis;
using ComfyBot.Data.Wrappers;
using LiteDB;
using System;

namespace ComfyBot.Data.Database;

[Obsolete]
[ExcludeFromCodeCoverage]
public class DatabaseFactory : IDatabaseFactory
{
    public IDatabase Create()
    {
        string databasePath = @"D:\Data\comfydeveloper - Copy.comfy";


        return new DatabaseWrapper(new LiteDatabase($"Filename={databasePath}; Connection=Shared;"));
    }
}