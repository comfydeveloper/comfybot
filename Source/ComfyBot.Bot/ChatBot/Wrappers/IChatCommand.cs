namespace ComfyBot.Bot.ChatBot.Wrappers
{
    using System.Collections.Generic;

    public interface IChatCommand
    {
        List<string> ArgumentsAsList { get; }

        string ArgumentsAsString { get; }

        IChatMessage ChatMessage { get; }

        string CommandText { get; }
    }
}