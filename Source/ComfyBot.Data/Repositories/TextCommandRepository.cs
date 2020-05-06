namespace ComfyBot.Data.Repositories
{
    using ComfyBot.Data.Database;
    using ComfyBot.Data.Models;

    public class TextCommandRepository : Repository<TextCommand>
    {
        public TextCommandRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory, "textCommands")
        { }

        protected override void Update(TextCommand source, TextCommand target)
        {
            target.Command = source.Command;
            target.TimeoutInSeconds = source.TimeoutInSeconds;
            target.LastUsed = source.LastUsed ?? target.LastUsed;
            target.Replies.Clear();
            target.Replies.AddRange(source.Replies);
        }
    }
}