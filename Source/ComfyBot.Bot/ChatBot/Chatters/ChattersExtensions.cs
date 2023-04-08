using System.Collections.Generic;
using System.Linq;

namespace ComfyBot.Bot.ChatBot.Chatters;

public static class ChattersExtensions
{
    public static IEnumerable<string> All(this Chatters chatters)
    {
        return chatters.Broadcaster
            .Concat(chatters.Moderators)
            .Concat(chatters.Viewers)
            .Concat(chatters.Vips)
            .Concat(chatters.Staff);
    }
}