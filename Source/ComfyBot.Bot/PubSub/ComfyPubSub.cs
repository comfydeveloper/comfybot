namespace ComfyBot.Bot.PubSub
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using ComfyBot.Bot.Initialization;
    using ComfyBot.Bot.PubSub.Extensions;
    using ComfyBot.Bot.PubSub.RewardRedeems;
    using ComfyBot.Bot.PubSub.Wrappers;
    using ComfyBot.Settings;

    using TwitchLib.Client.Interfaces;
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

            this.client = new TwitchPubSub();

            this.client.OnPubSubServiceConnected += this.ClientOnOnPubSubServiceConnected;
            this.client.OnRewardRedeemed += this.ClientOnOnRewardRedeemed;

            this.client.ListenToRewards(ApplicationSettings.Default.ChannelId);
            this.client.Connect();
        }

        private void ClientOnOnPubSubServiceConnected(object? sender, EventArgs e)
        {
            this.client.SendTopics();
        }

        private void ClientOnOnRewardRedeemed(object? sender, OnRewardRedeemedArgs e)
        {
            IRewardRedemption rewardRedemption = e.ToRewardRedemption();
            foreach (IRewardRedeemHandler rewardRedeemHandler in this.rewardRedeemHandlers)
            {
                rewardRedeemHandler.Handle(rewardRedemption);
            }
        }
    }
}
