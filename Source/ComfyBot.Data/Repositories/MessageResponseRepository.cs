namespace ComfyBot.Data.Repositories
{
    using ComfyBot.Data.Database;
    using ComfyBot.Data.Models;

    public class MessageResponseRepository : Repository<MessageResponse>
    {
        public MessageResponseRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory, "messageResponses")
        { }

        protected override void Update(MessageResponse source, MessageResponse target)
        {
            this.Clear(target);
            this.UpdateInternal(source, target);
        }

        private void Clear(MessageResponse target)
        {
            target.Users.Clear();
            target.AllKeywords.Clear();
            target.ExactKeywords.Clear();
            target.LooseKeywords.Clear();
            target.Replies.Clear();
        }

        private void UpdateInternal(MessageResponse source, MessageResponse target)
        {
            target.TimeoutInSeconds = source.TimeoutInSeconds;
            target.LastUsed = source.LastUsed ?? target.LastUsed;
            target.Users.AddRange(source.Users);
            target.ExactKeywords.AddRange(source.ExactKeywords);
            target.AllKeywords.AddRange(source.AllKeywords);
            target.LooseKeywords.AddRange(source.LooseKeywords);
            target.Replies.AddRange(source.Replies);
        }
    }
}