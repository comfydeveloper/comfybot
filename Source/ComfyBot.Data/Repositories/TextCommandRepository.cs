namespace ComfyBot.Data.Repositories
{
    using System;

    using Database;
    using Models;

    public class TextCommandRepository : Repository<TextCommand>
    {
        public TextCommandRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory, "textCommands")
        { }

        protected override void Update(TextCommand source, TextCommand target)
        {
            target.TimeoutInSeconds = source.TimeoutInSeconds;
            target.UseCount = Math.Max(source.UseCount, target.UseCount);
            target.LastUsed = source.LastUsed ?? target.LastUsed;
            target.Commands.Clear();
            target.Commands.AddRange(source.Commands);
            target.Replies.Clear();
            target.Replies.AddRange(source.Replies);
        }
    }
}