namespace ComfyBot.Bot.PubSub.Wrappers;

using System;
using System.Diagnostics.CodeAnalysis;

using TwitchLib.PubSub.Events;

[ExcludeFromCodeCoverage]
public class OnChannelPointsRewardRedeemedArgsWrapper : IRewardRedemption
{
    private readonly OnChannelPointsRewardRedeemedArgs onRewardRedeemedArgs;

    public OnChannelPointsRewardRedeemedArgsWrapper(OnChannelPointsRewardRedeemedArgs onRewardRedeemedArgs)
    {
        this.onRewardRedeemedArgs = onRewardRedeemedArgs;
    }

    public Guid RewardId => Guid.Parse(onRewardRedeemedArgs.RewardRedeemed.Redemption.Id);

    public string DisplayName => onRewardRedeemedArgs.RewardRedeemed.Redemption.User.DisplayName;

    public string Message => onRewardRedeemedArgs.RewardRedeemed.Redemption.UserInput;

    public string RewardTitle => onRewardRedeemedArgs.RewardRedeemed.Redemption.Reward.Title;

    public string RewardPrompt => onRewardRedeemedArgs.RewardRedeemed.Redemption.Reward.Prompt;

    public int RewardCost => onRewardRedeemedArgs.RewardRedeemed.Redemption.Reward.Cost;
}