using System;
using System.Diagnostics.CodeAnalysis;
using TwitchLib.PubSub.Events;

namespace ComfyBot.Bot.PubSub.Wrappers;

[ExcludeFromCodeCoverage]
public class OnChannelPointsRewardRedeemedArgsWrapper : IRewardRedemption
{
    private readonly OnChannelPointsRewardRedeemedArgs onRewardRedeemedArgs;

    public OnChannelPointsRewardRedeemedArgsWrapper(OnChannelPointsRewardRedeemedArgs onRewardRedeemedArgs)
    {
        this.onRewardRedeemedArgs = onRewardRedeemedArgs;
    }

    public Guid RewardId => Guid.Parse(this.onRewardRedeemedArgs.RewardRedeemed.Redemption.Id);

    public string DisplayName => this.onRewardRedeemedArgs.RewardRedeemed.Redemption.User.DisplayName;

    public string Message => this.onRewardRedeemedArgs.RewardRedeemed.Redemption.UserInput;

    public string RewardTitle => this.onRewardRedeemedArgs.RewardRedeemed.Redemption.Reward.Title;

    public string RewardPrompt => this.onRewardRedeemedArgs.RewardRedeemed.Redemption.Reward.Prompt;

    public int RewardCost => this.onRewardRedeemedArgs.RewardRedeemed.Redemption.Reward.Cost;
}