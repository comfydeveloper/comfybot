namespace ComfyBot.Bot.ChatBot.Wrappers;

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

    public bool IsBroadcaster => message.IsBroadcaster;

    public bool IsModerator => message.IsModerator;

    public string UserName => message.Username;

    public string Text => message.Message;
}