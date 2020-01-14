namespace ComfyBot.Data.Wrappers
{
    using System;

    public interface IDatabase : IDisposable
    {
        ILiteCollection<T> GetCollection<T>(string name);
    }
}