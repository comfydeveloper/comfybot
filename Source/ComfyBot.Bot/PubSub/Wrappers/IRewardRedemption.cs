namespace ComfyBot.Bot.PubSub.Wrappers
{
    using System;

    public interface IRewardRedemption
    {
        public Guid RewardId { get; }

        public string DisplayName { get; }

        public string Message { get; }

        public string RewardTitle { get; }

        public string RewardPrompt { get; }

        public int RewardCost { get; }
    }
}