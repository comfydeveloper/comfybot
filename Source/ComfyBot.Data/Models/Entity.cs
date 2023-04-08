using System;

namespace ComfyBot.Data.Models;

public abstract class Entity
{
    public string Id { get; set; }

    public DateTime DateOfCreation { get; set; }
}