namespace ComfyBot.Application.Bot.Wrappers
{
    using System.Collections.Generic;

    using TwitchLib.Client.Models;

    public interface IChatCommand
    {
        List<string> ArgumentsAsList { get; }

        string ArgumentsAsString { get; }

        IChatMessage ChatMessage { get; }

        string CommandText { get; }
    }
}