namespace ComfyBot.Data.Repositories;

using System;

using Database;
using Models;

public class RewardRedeemResponseRepository : Repository<RewardRedeemResponse>
{
    public RewardRedeemResponseRepository(IDatabaseFactory databaseFactory)
        : base(databaseFactory, "rewardRedeemResponses")
    { }

    protected override void Update(RewardRedeemResponse source, RewardRedeemResponse target)
    {
        Clear(target);
        UpdateInternal(source, target);
    }

    private static void Clear(RewardRedeemResponse target)
    {
        target.Replies.Clear();
    }

    private static void UpdateInternal(RewardRedeemResponse source, RewardRedeemResponse target)
    {
        target.RewardTitle = source.RewardTitle;
        target.Replies.AddRange(source.Replies);
        target.TimeoutInSeconds = source.TimeoutInSeconds;
        target.UseCount = Math.Max(source.UseCount, target.UseCount);
        target.LastUsed = source.LastUsed ?? target.LastUsed;
    }
}