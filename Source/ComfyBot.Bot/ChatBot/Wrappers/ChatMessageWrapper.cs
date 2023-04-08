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

    public bool IsBroadcaster => message.IsBroadcaster;

    public bool IsModerator => message.IsModerator;

    public string UserName => message.Username;

    public string Text => message.Message;
}