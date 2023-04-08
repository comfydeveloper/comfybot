using System;
using LiteDB;

namespace ComfyBot.Data.Wrappers;

public interface IDatabase : IDisposable
{
    ILiteCollection<T> GetCollection<T>(string name);
}