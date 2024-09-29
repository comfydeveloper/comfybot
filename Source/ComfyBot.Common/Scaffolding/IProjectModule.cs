using Microsoft.Extensions.DependencyInjection;

namespace ComfyBot.Common.Scaffolding;

public interface IProjectModule
{
    void RegisterServices(IServiceCollection services);
}