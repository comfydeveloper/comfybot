namespace ComfyBot.Bot.PubSub.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Bot.PubSub.Wrappers;

    using TwitchLib.PubSub.Events;

    [ExcludeFromCodeCoverage]
    public static class ArgsExtensions
    {
        public static IRewardRedemption ToRewardRedemption(this OnRewardRedeemedArgs args)
        {
            return new RewardRedeemedArgsWrapper(args);
        }
    }
}