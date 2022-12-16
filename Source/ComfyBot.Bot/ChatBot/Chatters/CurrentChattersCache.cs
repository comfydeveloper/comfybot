namespace ComfyBot.Bot.ChatBot.Chatters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CurrentChattersCache : IChattersCache
    {
        private static readonly List<Chatter> currentUsers = new();
        private static readonly Random random = new();

        public void Add(string user)
        {
            if (currentUsers.All(u => u.Name != user))
            {
                Chatter activity = new Chatter { Name = user };
                currentUsers.Add(activity);
            }
        }

        public void AddRange(IEnumerable<string> users)
        {
            foreach (string user in users)
            {
                Add(user);
            }
        }

        public void Remove(string user)
        {
            Chatter activity = currentUsers.FirstOrDefault(u => u.Name == user);
            currentUsers.Remove(activity);
        }

        public string GetRandom()
        {
            if (currentUsers.Any())
            {
                int randomIndex = random.Next(0, currentUsers.Count);
                return currentUsers[randomIndex].Name;
            }
            return string.Empty;
        }

        public IEnumerable<Chatter> GetAll()
        {
            return currentUsers.ToList();
        }

        public void UpdateActivity(string user)
        {
            Chatter activity = currentUsers.FirstOrDefault(u => u.Name == user);

            if (activity != null)
            {
                activity.LastActivity = DateTime.Now;
            }
        }
    }
}