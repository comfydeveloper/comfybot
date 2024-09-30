using System;
using System.Collections.Generic;

namespace ComfyBot.Data.Models;

public class MessageResponseOld : EntityOld
{
    public List<string> Users { get; set; } = [];

    public List<string> LooseKeywords { get; set; } = [];

    public List<string> AllKeywords { get; set; } = [];

    public List<string> ExactKeywords { get; set; } = [];

    public List<string> Replies { get; set; } = [];

    public DateTime? LastUsed { get; set; }

    public int TimeoutInSeconds { get; set; } = 30;

    public int UseCount { get; set; }

    public int Priority { get; set; }

    public bool ReplyAlways { get; set; }
}