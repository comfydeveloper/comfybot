namespace ComfyBot.Application.Bot.Wrappers
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using TwitchLib.Client.Models;

    [ExcludeFromCodeCoverage]
    public class ChatCommandWrapper : IChatCommand
    {
        private readonly ChatCommand command;

        public ChatCommandWrapper(ChatCommand command)
        {
            this.command = command;
        }

        public List<string> ArgumentsAsList { get => this.command.ArgumentsAsList; }

        public string ArgumentsAsString { get => this.command.ArgumentsAsString; }

        public ChatMessage ChatMessage { get => this.command.ChatMessage; }

        public string CommandText { get => this.command.CommandText; }
    }
}