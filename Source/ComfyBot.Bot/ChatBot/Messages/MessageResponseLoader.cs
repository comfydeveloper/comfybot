using System;
using System.Linq;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Bot.Extensions;
using ComfyBot.Data.Models;

namespace ComfyBot.Bot.ChatBot.Messages;

public class MessageResponseLoader : IMessageResponseLoader
{
    private readonly IWildcardReplacer wildcardReplacer;

    public MessageResponseLoader(IWildcardReplacer wildcardReplacer)
    {
        this.wildcardReplacer = wildcardReplacer;
    }

    public bool TryGetResponse(MessageResponse response, IChatMessage message, out string responseText)
    {
        responseText = null;

        if (HasOngoingTimeout(response))
        {
            return false;
        }

        if (response.Users.Any() && response.Users.Any(u => !string.Equals(u, message.UserName, StringComparison.CurrentCultureIgnoreCase)))
        {
            return false;
        }

        if (response.AlwaysReply || MatchesLooseKeyword(response, message) || MatchesAllKeywords(response, message) || MatchesExactKeyword(response, message))
        {
            responseText = response.Replies.GetRandom();
            responseText = responseText.Replace("{{user}}", message.UserName);
            responseText = this.wildcardReplacer.Replace(responseText);
            return true;
        }
        return false;
    }

    private static bool HasOngoingTimeout(MessageResponse response)
    {
        return response.LastUsedAt.HasValue && response.LastUsedAt > DateTime.Now.AddSeconds(-response.TimeoutInSeconds);
    }

    private static bool MatchesLooseKeyword(MessageResponse response, IChatMessage message)
    {
        foreach (string keyword in response.LooseKeywords)
        {
            if (message.Text.ToLower().Contains(keyword.ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private static bool MatchesAllKeywords(MessageResponse response, IChatMessage message)
    {
        if (!response.AllKeywords.Any())
        {
            return false;
        }

        foreach (string keyword in response.AllKeywords)
        {
            if (!message.Text.ToLower().Contains(keyword.ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }
        return true;
    }

    private static bool MatchesExactKeyword(MessageResponse response, IChatMessage message)
    {
        foreach (string keyword in response.ExactKeywords)
        {
            if (string.Equals(message.Text, keyword, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}