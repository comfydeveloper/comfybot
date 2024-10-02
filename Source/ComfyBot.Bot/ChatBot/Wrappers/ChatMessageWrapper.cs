using System.Diagnostics.CodeAnalysis;
using TwitchLib.Client.Models;

namespace ComfyBot.Bot.ChatBot.Wrappers;

[ExcludeFromCodeCoverage]
public class ChatMessageWrapper : IChatMessage
{
    private readonly ChatMessage message;

    public ChatMessageWrapper(ChatMessage message)
    {
        this.message = message;
    }

    public bool IsBroadcaster => this.message.IsBroadcaster;

    public bool IsModerator => this.message.IsModerator;

    public string UserName => this.message.Username;

    public string Text => this.message.Message;
}