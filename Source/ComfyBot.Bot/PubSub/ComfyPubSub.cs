namespace ComfyBot.Bot.PubSub
{
    using System;
    using System.Collections.Generic;
    using Extensions;
    using RewardRedeems;
    using Wrappers;
    using Settings;
    using TwitchLib.PubSub;
    using TwitchLib.PubSub.Events;

    public class ComfyPubSub : IComfyPubSub
    {
        private readonly IEnumerable<IRewardRedeemHandler> rewardRedeemHandlers;

        private TwitchPubSub client;

        public ComfyPubSub(IEnumerable<IRewardRedeemHandler> rewardRedeemHandlers)
        {
            this.rewardRedeemHandlers = rewardRedeemHandlers;
        }

        public void Run()
        {
            if (string.IsNullOrEmpty(ApplicationSettings.Default.ChannelId))
            {
                return;
            }

            client = new TwitchPubSub();

            client.OnPubSubServiceConnected += ClientOnOnPubSubServiceConnected;
            client.OnRewardRedeemed += ClientOnOnRewardRedeemed;

            client.ListenToRewards(ApplicationSettings.Default.ChannelId);
            client.Connect();
        }

        private void ClientOnOnPubSubServiceConnected(object? sender, EventArgs e)
        {
            client.SendTopics();
        }

        private void ClientOnOnRewardRedeemed(object? sender, OnRewardRedeemedArgs e)
        {
            IRewardRedemption rewardRedemption = e.ToRewardRedemption();
            foreach (IRewardRedeemHandler rewardRedeemHandler in rewardRedeemHandlers)
            {
                rewardRedeemHandler.Handle(rewardRedemption);
            }
        }
    }
}
