using System.Collections.Generic;
using System;

namespace ComfyBot.Data.Models;

public class TextCommand : Entity
{
    public required List<string> Replies { get; set; } = [];

    public required List<string> Commands { get; set; } = [];

    public required DateTime? LastUsedAt { get; set; }

    public required int UseCount { get; set; }

    public required int TimeoutInSeconds { get; set; }

    public void UpdateLastUsage()
    {
        this.UseCount++;
        this.LastUsedAt = DateTime.Now;
    }
}