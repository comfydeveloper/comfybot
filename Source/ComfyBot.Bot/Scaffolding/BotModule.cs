using ComfyBot.Bot.ChatBot;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Bot.ChatBot.Messages;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Services.Strategies;
using ComfyBot.Bot.ChatBot.Timezones;
using ComfyBot.Bot.Initialization;
using ComfyBot.Bot.PubSub;
using ComfyBot.Bot.PubSub.RewardRedeems;
using ComfyBot.Common.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.Scaffolding;

public class BotModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddAllImplementing(typeof(ICommandHandler), ServiceLifetime.Transient);
        services.AddAllImplementing(typeof(IChatMessageHandler), ServiceLifetime.Transient);
        services.AddAllImplementing(typeof(IRewardRedeemHandler), ServiceLifetime.Transient);
        services.AddAllImplementing(typeof(IReplacementStrategy), ServiceLifetime.Transient);

        services.AddTransient<ITimezoneLoader, TimezoneLoader>();
        services.AddTransient<ITimeLoader, TimeLoader>();

        services.AddTransient<ITextCommandReplyLoader, TextCommandReplyLoader>();
        services.AddTransient<IMessageResponseLoader, MessageResponseLoader>();

        services.AddTransient<IWildcardReplacer, WildcardReplacer>();

        services.AddTransient<IComfyBot, ChatBot.ChatBot>();
        services.AddTransient<IComfyPubSub, ComfyPubSub>();

        services.AddTransient<ITwitchClientFactory, TwitchClientFactory>();

    }

    public void Configure(IHost applicationBuilder)
    {
    }
}