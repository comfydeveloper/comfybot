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

    public List<string> ArgumentsAsList => this.command.ArgumentsAsList;

    public string ArgumentsAsString => this.command.ArgumentsAsString;

    public IChatMessage ChatMessage => new ChatMessageWrapper(this.command.ChatMessage);

    public string CommandText => this.command.CommandText;

    public bool IsBroadcaster => this.command.ChatMessage.IsBroadcaster;

    public bool IsModerator => this.command.ChatMessage.IsModerator;
}