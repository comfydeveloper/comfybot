using System;
using System.Collections.Generic;

namespace ComfyBot.Data.Models;

public class TextCommand : Entity
{
    public List<string> Replies { get; set; } = [];

    public List<string> Commands { get; set; } = [];

    public DateTime? LastUsed { get; set; }

    public int UseCount { get; set; }

    public int TimeoutInSeconds { get; set; }
}