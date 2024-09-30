using System;

namespace ComfyBot.Data.Models;

public abstract class Entity
{
    public required Guid Id { get; init; }

    public required DateTime CreatedAt { get; init; } = DateTime.Now;
}