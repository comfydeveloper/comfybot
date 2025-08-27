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

namespace ComfyBot.Bot.Scaffolding;

public class BotModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddAllImplementing(typeof(ICommandHandler), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(IChatMessageHandler), ServiceLifetime.Scoped);
        services.AddAllImplementing(typeof(IRewardRedeemHandler), ServiceLifetime.Transient);
        services.AddAllImplementing(typeof(IReplacementStrategy), ServiceLifetime.Scoped);

        services.AddTransient<ITimezoneLoader, TimezoneLoader>();
        services.AddTransient<ITimeLoader, TimeLoader>();

        services.AddTransient<ITextCommandReplyLoader, TextCommandReplyLoader>();
        services.AddTransient<IMessageResponseLoader, MessageResponseLoader>();

        services.AddTransient<IWildcardReplacer, WildcardReplacer>();

        services.AddSingleton<IMessageSender, MessageSender>();

        services.AddTransient<IComfyBot, ChatBot.ChatBot>();
        services.AddTransient<IComfyPubSub, ComfyPubSub>();

        services.AddTransient<ITwitchClientFactory, TwitchClientFactory>();

        services.AddSingleton(serviceProvider =>
        {
            ITwitchClientFactory factory = serviceProvider.GetRequiredService<ITwitchClientFactory>();

            return factory.Create();
        });
    }

    public void Configure(IHost applicationBuilder)
    {
    }
}