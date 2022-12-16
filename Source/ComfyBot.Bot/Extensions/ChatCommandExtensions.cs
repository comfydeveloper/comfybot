namespace ComfyBot.Bot.Extensions;

using ChatBot.Wrappers;

public static class ChatCommandExtensions
{
    public static bool HasParameters(this IChatCommand command, int minimumParameters = 1)
    {
        return command.ArgumentsAsList.Count >= minimumParameters;
    }

    public static bool Is(this IChatCommand command, string commandText)
    {
        return command.CommandText.ToLower() == commandText;
    }

    public static bool IsFromStaff(this IChatCommand command)
    {
        return command.IsBroadcaster || command.IsModerator;
    }
}