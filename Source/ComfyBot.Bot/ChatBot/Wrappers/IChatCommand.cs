using System.Collections.Generic;

namespace ComfyBot.Bot.ChatBot.Wrappers;

public interface IChatCommand
{
    List<string> ArgumentsAsList { get; }

    string ArgumentsAsString { get; }

    IChatMessage ChatMessage { get; }

    string CommandText { get; }

    bool IsBroadcaster { get; }

    bool IsModerator { get; }
}