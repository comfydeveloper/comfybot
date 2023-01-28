namespace ComfyBot.Bot.PubSub.Extensions;

using System.Diagnostics.CodeAnalysis;

using Wrappers;

using TwitchLib.PubSub.Events;

[ExcludeFromCodeCoverage]
public static class OnChannelPointsRewardRedeemedArgsExtensions
{
    public static IRewardRedemption ToRewardRedemption(this OnChannelPointsRewardRedeemedArgs args)
    {
        return new OnChannelPointsRewardRedeemedArgsWrapper(args);
    }
}