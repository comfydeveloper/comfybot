namespace ComfyBot.Data.Models;

using System;
using System.Collections.Generic;

public class RewardRedeemResponse : Entity
{
    public string RewardTitle { get; set; }

    public List<string> Replies { get; set; }

    public DateTime? LastUsed { get; set; }

    public int UseCount { get; set; }

    public int TimeoutInSeconds { get; set; }
}