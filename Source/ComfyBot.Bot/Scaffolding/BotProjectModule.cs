using ComfyBot.Common.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComfyBot.Bot.Scaffolding;

public class BotProjectModule : IProjectModule
{
    public void RegisterServices(IServiceCollection services)
    {
    }

    public void Configure(IHost applicationBuilder)
    {
    }
}