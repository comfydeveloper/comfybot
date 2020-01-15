namespace ComfyBot.Data.Repositories
{
    using ComfyBot.Data.Database;
    using ComfyBot.Data.Models;

    public class ShoutoutRepository : Repository<Shoutout>
    {
        public ShoutoutRepository(IDatabaseFactory databaseFactory) : base(databaseFactory, "shoutouts")
        { }

        protected override void Update(Shoutout source, Shoutout target)
        {
            target.Command = source.Command;
            target.Message = source.Message;
        }
    }
}