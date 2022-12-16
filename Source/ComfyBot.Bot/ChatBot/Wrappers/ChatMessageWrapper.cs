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

        public bool IsBroadcaster { get => message.IsBroadcaster; }

        public bool IsModerator { get => message.IsModerator; }

        public string UserName { get => message.Username; }

        public string Text { get => message.Message; }
    }
}