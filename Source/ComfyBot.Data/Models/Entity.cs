using System;

namespace ComfyBot.Data.Models;

public abstract class Entity
{
    public required Guid Id { get; set; }

    public required DateTime CreatedAt { get; set; }
}