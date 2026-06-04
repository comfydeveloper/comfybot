using ComfyBot.Gateway.Configuration;
using ComfyBot.Gateway.Services;

namespace ComfyBot.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GatewaySettings>(configuration.GetSection(GatewaySettings.SectionName));

        services.AddSingleton<ITwitchService, TwitchService>();
        services.AddSingleton<IRedisService, RedisService>();

        GatewaySettings? gatewaySettings = configuration.GetSection(GatewaySettings.SectionName).Get<GatewaySettings>();
        if (gatewaySettings != null && !string.IsNullOrWhiteSpace(gatewaySettings.RedisConnectionString))
        {
            services.AddSignalR()
                .AddStackExchangeRedis(gatewaySettings.RedisConnectionString, options =>
                {
                    options.Configuration.ChannelPrefix = "ComfyBot.Gateway";
                });
        }
        else
        {
            services.AddSignalR();
        }

        return services;
    }
}
