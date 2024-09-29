using System;
using System.Collections.Generic;
using ComfyBot.Bot.PubSub.Extensions;
using ComfyBot.Bot.PubSub.RewardRedeems;
using ComfyBot.Bot.PubSub.Wrappers;
using ComfyBot.Settings;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace ComfyBot.Bot.PubSub;

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
        this.client.OnChannelPointsRewardRedeemed += this.OnChannelPointsRewardRedeemed;

        this.client.ListenToChannelPoints(ApplicationSettings.Default.ChannelId);
        this.client.Connect();
    }

    private void ClientOnOnPubSubServiceConnected(object sender, EventArgs e)
    {
        this.client.SendTopics();
    }

    private void OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e)
    {
        try
        {
            IRewardRedemption rewardRedemption = e.ToRewardRedemption();
            foreach (IRewardRedeemHandler rewardRedeemHandler in this.rewardRedeemHandlers)
            {
                rewardRedeemHandler.Handle(rewardRedemption);
            }
        }
        catch (Exception ex)
        {
            Log($"Failed to handle channel point redeem {e.RewardRedeemed.Redemption.Reward.Title} - {ex.Message}");
        }
    }

    private static void Log(string message)
    {
        Console.Write($"{DateTime.Now}: {message}");
    }
}