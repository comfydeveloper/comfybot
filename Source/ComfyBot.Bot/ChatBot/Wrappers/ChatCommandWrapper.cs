using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TwitchLib.Client.Models;

namespace ComfyBot.Bot.ChatBot.Wrappers;

[ExcludeFromCodeCoverage]
public class ChatCommandWrapper : IChatCommand
{
    private readonly ChatCommand command;

    public ChatCommandWrapper(ChatCommand command)
    {
        this.command = command;
    }

    public List<string> ArgumentsAsList => command.ArgumentsAsList;

    public string ArgumentsAsString => command.ArgumentsAsString;

    public IChatMessage ChatMessage => new ChatMessageWrapper(command.ChatMessage);

    public string CommandText => command.CommandText;

    public bool IsBroadcaster => command.ChatMessage.IsBroadcaster;

    public bool IsModerator => command.ChatMessage.IsModerator;
}