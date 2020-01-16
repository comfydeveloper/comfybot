namespace ComfyBot.Bot.ChatBot.Wrappers
{
    using System.Diagnostics.CodeAnalysis;

    using TwitchLib.Client.Models;

    [ExcludeFromCodeCoverage]
    public class ChatMessageWrapper : IChatMessage
    {
        private readonly ChatMessage message;

        public ChatMessageWrapper(ChatMessage message)
        {
            this.message = message;
        }

        public bool IsBroadcaster { get => this.message.IsBroadcaster; }

        public bool IsModerator { get => this.message.IsModerator; }

        public string UserName { get => this.message.Username; }

        public string Text { get => this.message.Message; }
    }
}