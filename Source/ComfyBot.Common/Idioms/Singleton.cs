﻿using System.Diagnostics.CodeAnalysis;

namespace ComfyBot.Common.Idioms;

[ExcludeFromCodeCoverage]
public abstract class Singleton<TInterface, TImplementation>
    where TInterface : class
    where TImplementation : TInterface, new()
{
    private static readonly TImplementation instance = new();

    private static TInterface overriddenInstance;

    public static TInterface Instance => overriddenInstance ?? instance;

    public static void OverrideInstance(TInterface newInstance)
    {
        overriddenInstance = newInstance;
    }
}