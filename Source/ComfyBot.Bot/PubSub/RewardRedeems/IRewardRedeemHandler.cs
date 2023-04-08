using ComfyBot.Bot.PubSub.Wrappers;

namespace ComfyBot.Bot.PubSub.RewardRedeems;

public interface IRewardRedeemHandler
{
    public void Handle(IRewardRedemption redemption);
}