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
    private readonly IRepository<MessageResponse> repository;
    private readonly IWildcardReplacer wildcardReplacerObject;

    public MessageResponseLoader(IRepository<MessageResponse> repository, IWildcardReplacer wildcardReplacerObject)
    {
        this.repository = repository;
        this.wildcardReplacerObject = wildcardReplacerObject;
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

        if (response.ReplyAlways || MatchesLooseKeyword(response, message) || MatchesAllKeywords(response, message) || MatchesExactKeyword(response, message))
        {
            UpdateUsageInfo(response);
            responseText = response.Replies.GetRandom();
            responseText = responseText.Replace("{{user}}", message.UserName);
            responseText = wildcardReplacerObject.Replace(responseText);
            return true;
        }
        return false;
    }

    private void UpdateUsageInfo(MessageResponse response)
    {
        response.UseCount++;
        response.LastUsed = DateTime.Now;
        repository.Write(response);
    }

    private static bool HasOngoingTimeout(MessageResponse response)
    {
        return response.LastUsed.HasValue && response.LastUsed > DateTime.Now.AddSeconds(-response.TimeoutInSeconds);
    }

    private static bool MatchesLooseKeyword(MessageResponse messageResponse, IChatMessage message)
    {
        foreach (string keyword in messageResponse.LooseKeywords)
        {
            if (message.Text.ToLower().Contains(keyword.ToLower()))
            {
                return true;
            }
        }
        return false;
    }

    private static bool MatchesAllKeywords(MessageResponse messageResponse, IChatMessage message)
    {
        if (!messageResponse.AllKeywords.Any())
        {
            return false;
        }

        foreach (string keyword in messageResponse.AllKeywords)
        {
            if (!message.Text.ToLower().Contains(keyword.ToLower()))
            {
                return false;
            }
        }
        return true;
    }

    private static bool MatchesExactKeyword(MessageResponse messageResponse, IChatMessage message)
    {
        foreach (string keyword in messageResponse.ExactKeywords)
        {
            if (string.Equals(message.Text, keyword, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}