namespace ComfyBot.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class TextCommand : Entity
    {
        public List<string> Replies { get; set; } = new();

        public List<string> Commands { get; set; } = new();

        public DateTime? LastUsed { get; set; }

        public int UseCount { get; set; }

        public int TimeoutInSeconds { get; set; }
    }
}