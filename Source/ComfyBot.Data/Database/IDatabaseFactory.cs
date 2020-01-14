namespace ComfyBot.Data.Database
{
    using ComfyBot.Data.Wrappers;

    public interface IDatabaseFactory
    {
        IDatabase Create();
    }
}