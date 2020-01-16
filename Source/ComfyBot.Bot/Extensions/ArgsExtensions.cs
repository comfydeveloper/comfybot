namespace ComfyBot.Bot.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Bot.ChatBot.Wrappers;

    using TwitchLib.Client.Models;

    [ExcludeFromCodeCoverage]
    public static class ArgsExtensions
    {
        public static IChatCommand Wrap(this ChatCommand command)
        {
            return new ChatCommandWrapper(command);
        }

        public static IChatMessage Wrap(this ChatMessage message)
        {
            return new ChatMessageWrapper(message);
        }
    }
}