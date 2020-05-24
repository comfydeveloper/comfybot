namespace ComfyBot.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class MessageResponse : Entity
    {
        public List<string> Users { get; set; } = new List<string>();

        public List<string> LooseKeywords { get; set; } = new List<string>();

        public List<string> AllKeywords { get; set; } = new List<string>();

        public List<string> ExactKeywords { get; set; } = new List<string>();

        public List<string> Replies { get; set; } = new List<string>();

        public DateTime? LastUsed { get; set; }

        public int TimeoutInSeconds { get; set; } = 30;

        public int UseCount { get; set; }
    }
}