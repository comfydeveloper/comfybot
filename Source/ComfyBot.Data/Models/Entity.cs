using System;

namespace ComfyBot.Data.Models;

public abstract class Entity
{
    public required Guid Id { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}