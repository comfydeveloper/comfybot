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

        public Guid RewardId => onRewardRedeemedArgs.RewardId;

        public string DisplayName => onRewardRedeemedArgs.DisplayName;

        public string Message => onRewardRedeemedArgs.Message;

        public string RewardTitle => onRewardRedeemedArgs.RewardTitle;

        public string RewardPrompt => onRewardRedeemedArgs.RewardPrompt;

        public int RewardCost => onRewardRedeemedArgs.RewardCost;
    }
}