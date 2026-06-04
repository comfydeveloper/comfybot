using ComfyBot.Gateway.Contracts.Models;
using System;
using System.Linq;

namespace ComfyBot.Bot.ChatBot.Messages.Extensions;

public static class ChatMessageExtensions
{
    public static bool StartsWith(this IChatMessage message, string text)
    {
        return message.Text.StartsWith(text);
    }

    public static bool IsCommand(this IChatMessage message)
    {
        return message.Text.StartsWith('!');
    }

    public static bool SentBy(this IChatMessage message, params string[] userNames)
    {
        return userNames.Any(n => n.Equals(message.UserName, StringComparison.OrdinalIgnoreCase));
    }
}