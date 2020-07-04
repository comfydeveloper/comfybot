namespace ComfyBot.Bot.ChatBot.Chatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CurrentChattersCache : IChattersCache
    {
        private static readonly List<string> currentUsers = new List<string>();
        private static readonly Random random = new Random();

        public void Add(string user)
        {
            if (!currentUsers.Contains(user))
            {
                currentUsers.Add(user);
            }
        }

        public void AddRange(IEnumerable<string> users)
        {
            foreach (string user in users)
            {
                this.Add(user);
            }
        }

        public void Remove(string user)
        {
            currentUsers.Remove(user);
        }

        public string GetRandom()
        {
            if (currentUsers.Any())
            {
                int randomIndex = random.Next(0, currentUsers.Count);
                return currentUsers[randomIndex];
            }
            return string.Empty;
        }

        public IEnumerable<string> GetAll()
        {
            return currentUsers.ToList();
        }
    }
}