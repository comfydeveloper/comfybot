namespace ComfyBot.Bot.PubSub.RewardRedeems
{
    using ComfyBot.Bot.PubSub.Wrappers;

    public interface IRewardRedeemHandler
    {
        public void Handle(IRewardRedemption redemption);
    }
}