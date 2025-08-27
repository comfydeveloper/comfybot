namespace ComfyBot.Application.Shared.Services;

public interface IScopedServiceProvider
{
    ScopedService<T> Create<T>();
}