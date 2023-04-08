using System.Diagnostics.CodeAnalysis;
using ComfyBot.Bot.PubSub.Wrappers;
using TwitchLib.PubSub.Events;

namespace ComfyBot.Bot.PubSub.Extensions;

[ExcludeFromCodeCoverage]
public static class OnChannelPointsRewardRedeemedArgsExtensions
{
    public static IRewardRedemption ToRewardRedemption(this OnChannelPointsRewardRedeemedArgs args)
    {
        return new OnChannelPointsRewardRedeemedArgsWrapper(args);
    }
}