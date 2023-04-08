using System;
using ComfyBot.Data.Database;
using ComfyBot.Data.Models;

namespace ComfyBot.Data.Repositories;

public class MessageResponseRepository : Repository<MessageResponse>
{
    public MessageResponseRepository(IDatabaseFactory databaseFactory)
        : base(databaseFactory, "messageResponses")
    { }

    protected override void Update(MessageResponse source, MessageResponse target)
    {
        target.Priority = source.Priority;
        target.UseCount = Math.Max(source.UseCount, target.UseCount);
        target.TimeoutInSeconds = source.TimeoutInSeconds;
        target.LastUsed = source.LastUsed ?? target.LastUsed;
        target.Users = source.Users;
        target.ExactKeywords = source.ExactKeywords;
        target.AllKeywords = source.AllKeywords;
        target.LooseKeywords = source.LooseKeywords;
        target.Replies = source.Replies;
        target.ReplyAlways = source.ReplyAlways;
    }
}