namespace ComfyBot.Bot.Extensions;

using System;
using System.Text.RegularExpressions;

public static class StringExtensions
{
    private const string RegularExpression = @"parameter(\d+)}}";

    public static bool CanHandleParameters(this string reply, int parameterCount)
    {
        MatchCollection matches = Regex.Matches(reply, RegularExpression);

        foreach (Match match in matches)
        {
            if (int.Parse(match.Groups[1].ToString()) > parameterCount)
            {
                return false;
            }
        }
        return true;
    }

    public static string ReplaceFirst(this string text, string search, string replace)
    {
        int position = text.IndexOf(search, StringComparison.Ordinal);
        if (position < 0)
        {
            return text;
        }
        return text.Substring(0, position) + replace + text.Substring(position + search.Length);
    }
}