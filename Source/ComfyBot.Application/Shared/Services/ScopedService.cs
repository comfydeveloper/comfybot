using Microsoft.Extensions.DependencyInjection;
using System;

namespace ComfyBot.Application.Shared.Services;

public class ScopedService<T> : IDisposable
{
    private readonly IServiceScope scope;

    public T Service { get; }

    public ScopedService(IServiceScope scope, T service)
    {
        this.Service = service;
        this.scope = scope;
    }

    public void Dispose()
    {
        this.scope?.Dispose();
    }
}