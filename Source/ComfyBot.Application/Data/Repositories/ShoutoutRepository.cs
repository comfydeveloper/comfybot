namespace ComfyBot.Application.Data.Repositories
{
    using ComfyBot.Application.Data.Models;

    public class ShoutoutRepository : Repository<Shoutout>
    {
        public ShoutoutRepository(IDatabaseFactory databaseFactory) : base(databaseFactory, "shoutouts")
        { }

        protected override void Update(Shoutout source, Shoutout target)
        {
            target.ShoutoutText = source.ShoutoutText;
        }
    }
}