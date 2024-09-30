using System;
using System.Linq;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Extensions;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;

namespace ComfyBot.Bot.ChatBot.Messages;

public class MessageResponseLoader : IMessageResponseLoader
{
    private readonly IRepository<MessageResponseOld> repository;
    private readonly IWildcardReplacer wildcardReplacerObject;

    public MessageResponseLoader(IRepository<MessageResponseOld> repository, IWildcardReplacer wildcardReplacerObject)
    {
        this.repository = repository;
        this.wildcardReplacerObject = wildcardReplacerObject;
    }

    public bool TryGetResponse(MessageResponseOld responseOld, IChatMessage message, out string responseText)
    {
        responseText = null;

        if (HasOngoingTimeout(responseOld))
        {
            return false;
        }

        if (responseOld.Users.Any() && responseOld.Users.Any(u => !string.Equals(u, message.UserName, StringComparison.CurrentCultureIgnoreCase)))
        {
            return false;
        }

        if (responseOld.ReplyAlways || MatchesLooseKeyword(responseOld, message) || MatchesAllKeywords(responseOld, message) || MatchesExactKeyword(responseOld, message))
        {
            this.UpdateUsageInfo(responseOld);
            responseText = responseOld.Replies.GetRandom();
            responseText = responseText.Replace("{{user}}", message.UserName);
            responseText = this.wildcardReplacerObject.Replace(responseText);
            return true;
        }
        return false;
    }

    private void UpdateUsageInfo(MessageResponseOld responseOld)
    {
        responseOld.UseCount++;
        responseOld.LastUsed = DateTime.Now;
        this.repository.Write(responseOld);
    }

    private static bool HasOngoingTimeout(MessageResponseOld responseOld)
    {
        return responseOld.LastUsed.HasValue && responseOld.LastUsed > DateTime.Now.AddSeconds(-responseOld.TimeoutInSeconds);
    }

    private static bool MatchesLooseKeyword(MessageResponseOld messageResponseOld, IChatMessage message)
    {
        foreach (string keyword in messageResponseOld.LooseKeywords)
        {
            if (message.Text.ToLower().Contains(keyword.ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private static bool MatchesAllKeywords(MessageResponseOld messageResponseOld, IChatMessage message)
    {
        if (!messageResponseOld.AllKeywords.Any())
        {
            return false;
        }

        foreach (string keyword in messageResponseOld.AllKeywords)
        {
            if (!message.Text.ToLower().Contains(keyword.ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }
        return true;
    }

    private static bool MatchesExactKeyword(MessageResponseOld messageResponseOld, IChatMessage message)
    {
        foreach (string keyword in messageResponseOld.ExactKeywords)
        {
            if (string.Equals(message.Text, keyword, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}