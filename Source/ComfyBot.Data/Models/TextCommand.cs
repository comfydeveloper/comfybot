namespace ComfyBot.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class TextCommand : Entity
    {
        public string Command { get; set; }

        public List<string> Replies { get; set; } = new List<string>();

        public DateTime? LastUsed { get; set; }

        public int TimeoutInSeconds { get; set; }
    }
}