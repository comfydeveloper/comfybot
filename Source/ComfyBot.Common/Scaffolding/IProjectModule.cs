using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ComfyBot.Common.Scaffolding;

public interface IProjectModule
{
    void RegisterServices(IServiceCollection services);

    void Configure(IHost applicationBuilder);
}