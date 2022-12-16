namespace ComfyBot.Data.Wrappers;

using LiteDB;
using System;

public interface IDatabase : IDisposable
{
    ILiteCollection<T> GetCollection<T>(string name);
}