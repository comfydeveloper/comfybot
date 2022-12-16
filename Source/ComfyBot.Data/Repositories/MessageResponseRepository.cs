namespace ComfyBot.Data.Repositories
{
    using System;

    using Database;
    using Models;

    public class MessageResponseRepository : Repository<MessageResponse>
    {
        public MessageResponseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory, "messageResponses")
        { }

        protected override void Update(MessageResponse source, MessageResponse target)
        {
            Clear(target);
            UpdateInternal(source, target);
        }

        private static void Clear(MessageResponse target)
        {
            target.Users.Clear();
            target.AllKeywords.Clear();
            target.ExactKeywords.Clear();
            target.LooseKeywords.Clear();
            target.Replies.Clear();
        }

        private static void UpdateInternal(MessageResponse source, MessageResponse target)
        {
            target.Priority = source.Priority;
            target.UseCount = Math.Max(source.UseCount, target.UseCount);
            target.TimeoutInSeconds = source.TimeoutInSeconds;
            target.LastUsed = source.LastUsed ?? target.LastUsed;
            target.Users.AddRange(source.Users);
            target.ExactKeywords.AddRange(source.ExactKeywords);
            target.AllKeywords.AddRange(source.AllKeywords);
            target.LooseKeywords.AddRange(source.LooseKeywords);
            target.Replies.AddRange(source.Replies);
            target.ReplyAlways = source.ReplyAlways;
        }
    }
}