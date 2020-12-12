namespace ComfyBot.Bot.PubSub.Wrappers
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using TwitchLib.PubSub.Events;

    [ExcludeFromCodeCoverage]
    public class RewardRedeemedArgsWrapper : IRewardRedemption
    {
        private readonly OnRewardRedeemedArgs onRewardRedeemedArgs;

        public RewardRedeemedArgsWrapper(OnRewardRedeemedArgs onRewardRedeemedArgs)
        {
            this.onRewardRedeemedArgs = onRewardRedeemedArgs;
        }

        public Guid RewardId => this.onRewardRedeemedArgs.RewardId;

        public string DisplayName => this.onRewardRedeemedArgs.DisplayName;

        public string Message => this.onRewardRedeemedArgs.Message;

        public string RewardTitle => this.onRewardRedeemedArgs.RewardTitle;

        public string RewardPrompt => this.onRewardRedeemedArgs.RewardPrompt;

        public int RewardCost => this.onRewardRedeemedArgs.RewardCost;
    }
}