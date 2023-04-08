using ComfyBot.Data.Wrappers;

namespace ComfyBot.Data.Database;

public interface IDatabaseFactory
{
    IDatabase Create();
}