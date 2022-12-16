namespace ComfyBot.Bot.PubSub.RewardRedeems;

using Wrappers;

public interface IRewardRedeemHandler
{
    public void Handle(IRewardRedemption redemption);
}