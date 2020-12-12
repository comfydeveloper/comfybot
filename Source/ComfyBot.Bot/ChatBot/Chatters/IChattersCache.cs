namespace ComfyBot.Bot.ChatBot.Chatters
{
    using System.Collections.Generic;

    public interface IChattersCache
    {
        public void Add(string user);

        public void AddRange(IEnumerable<string> users);

        public void Remove(string user);

        public string GetRandom();

        public void UpdateActivity(string user);

        public IEnumerable<Chatter> GetAll();
    }
}