using Microsoft.Extensions.DependencyInjection;
using System;

namespace ComfyBot.Application.Shared.Services;

public class ScopedServiceProvider : IScopedServiceProvider
{
    private readonly IServiceProvider serviceProvider;

    public ScopedServiceProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ScopedService<T> Create<T>()
    {
        IServiceScope serviceScope = this.serviceProvider.CreateScope();

        ScopedService<T> scopedService = new(serviceScope, serviceScope.ServiceProvider.GetRequiredService<T>());
        return scopedService;
    }
}