using ComfyBot.Common.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComfyBot.Common.Scaffolding;

public class CommonModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpService, HttpService>();
    }

    public void Configure(IHost applicationBuilder)
    {
    }
}