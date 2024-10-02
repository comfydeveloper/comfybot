using System.Collections.Generic;

namespace ComfyBot.Application.Features.Shared;

public abstract class ListDto<T>
{
    public List<T> Entries { get; init; } = new();
}