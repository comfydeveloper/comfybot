namespace ComfyBot.Data.Models
{
    using System;

    public abstract class Entity
    {
        public string Id { get; set; }

        public DateTime DateOfCreation { get; set; }
    }
}