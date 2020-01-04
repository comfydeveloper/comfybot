namespace ComfyBot.Application.Bot.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    using global::ComfyBot.Application.Bot.Wrappers;

    using TwitchLib.Client.Models;

    [ExcludeFromCodeCoverage]
    public static class ArgsExtensions
    {
        public static IChatCommand Wrap(this ChatCommand command)
        {
            return new ChatCommandWrapper(command);
        }
    }
}