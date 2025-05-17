using ComfyBot.Bot.ChatBot.Wrappers;

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
}