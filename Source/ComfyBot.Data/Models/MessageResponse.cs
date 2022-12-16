namespace ComfyBot.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class MessageResponse : Entity
    {
        public List<string> Users { get; set; } = new();

        public List<string> LooseKeywords { get; set; } = new();

        public List<string> AllKeywords { get; set; } = new();

        public List<string> ExactKeywords { get; set; } = new();

        public List<string> Replies { get; set; } = new();

        public DateTime? LastUsed { get; set; }

        public int TimeoutInSeconds { get; set; } = 30;

        public int UseCount { get; set; }

        public int Priority { get; set; }

        public bool ReplyAlways { get; set; }
    }
}