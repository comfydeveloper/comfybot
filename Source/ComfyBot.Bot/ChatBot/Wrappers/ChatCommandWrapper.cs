namespace ComfyBot.Bot.ChatBot.Wrappers;

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

    public List<string> ArgumentsAsList { get => command.ArgumentsAsList; }

    public string ArgumentsAsString { get => command.ArgumentsAsString; }

    public IChatMessage ChatMessage { get => new ChatMessageWrapper(command.ChatMessage); }

    public string CommandText { get => command.CommandText; }

    public bool IsBroadcaster { get => command.ChatMessage.IsBroadcaster; }

    public bool IsModerator { get => command.ChatMessage.IsModerator; }
}