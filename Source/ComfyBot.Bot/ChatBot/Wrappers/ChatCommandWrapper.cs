namespace ComfyBot.Bot.ChatBot.Wrappers
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

        public IChatMessage ChatMessage { get => new ChatMessageWrapper(this.command.ChatMessage); }

        public string CommandText { get => this.command.CommandText; }
    }
}