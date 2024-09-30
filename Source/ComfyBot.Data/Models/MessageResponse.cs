using System;
using System.Collections.Generic;

namespace ComfyBot.Data.Models;

public class MessageResponse : Entity
{
    public required List<string> Users { get; set; } = [];

    public required List<string> LooseKeywords { get; set; } = [];

    public required List<string> AllKeywords { get; set; } = [];

    public required List<string> ExactKeywords { get; set; } = [];

    public required List<string> Replies { get; set; } = [];

    public required DateTime? LastUsedAt { get; set; }

    public required int TimeoutInSeconds { get; set; } = 30;

    public required int UseCount { get; set; }

    public required int Priority { get; set; }

    public required bool AlwaysReply { get; set; }
}