namespace ComfyBot.Application.Data
{
    using ComfyBot.Application.Data.Wrappers;

    public interface IDatabaseFactory
    {
        IDatabase Create();
    }
}