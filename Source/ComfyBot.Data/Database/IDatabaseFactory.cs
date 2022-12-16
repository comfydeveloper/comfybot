namespace ComfyBot.Data.Database;

using Wrappers;

public interface IDatabaseFactory
{
    IDatabase Create();
}