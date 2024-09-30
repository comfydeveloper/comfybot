using System;
using System.Linq;
using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Extensions;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;

namespace ComfyBot.Bot.ChatBot.Commands;

public class TextCommandReplyLoader : ITextCommandReplyLoader
{
    private readonly IRepository<TextCommandOld> repository;
    private readonly IWildcardReplacer wildcardReplacer;

    public TextCommandReplyLoader(IRepository<TextCommandOld> repository, IWildcardReplacer wildcardReplacer)
    {
        this.repository = repository;
        this.wildcardReplacer = wildcardReplacer;
    }

    public bool TryGetReply(TextCommandOld textCommandOld, IChatCommand command, out string reply)
    {
        if (!this.HasOngoingTimeout(textCommandOld) && CommandMatches(textCommandOld, command))
        {
            this.UpdateCommandUsageInfo(textCommandOld);

            if (command.ArgumentsAsList.Any())
            {
                string[] repliesWithParameters = textCommandOld.Replies.Where(r => r.Contains("{{parameters}}")
                    || r.Contains("{{parameter") && r.CanHandleParameters(command.ArgumentsAsList.Count)).ToArray();

                //[TODO] try to match commands with *exactly* n parameters first
                if (repliesWithParameters.Any())
                {
                    reply = repliesWithParameters.GetRandom();
                    reply = reply.Replace("{{user}}", command.ChatMessage.UserName);
                    reply = reply.Replace("{{parameters}}", command.ArgumentsAsString);

                    var parametersWithIndexes = command.ArgumentsAsList.Select((s, i) => new { Text = s, Index = i });

                    foreach (var parameter in parametersWithIndexes)
                    {
                        reply = reply.Replace($"{{{{parameter{parameter.Index + 1}}}}}", parameter.Text);
                    }
                    reply = this.wildcardReplacer.Replace(reply, new WildcardReplacerOptions { Parameters = command.ArgumentsAsList.ToArray(), UserName = command.ChatMessage.UserName });
                    return true;
                }
            }
            reply = textCommandOld.Replies.Where(r => !r.Contains("{{parameter")).GetRandom();
            reply = reply.Replace("{{user}}", command.ChatMessage.UserName);
            reply = this.wildcardReplacer.Replace(reply);
            return true;
        }
        reply = null;
        return false;
    }

    private bool HasOngoingTimeout(TextCommandOld textCommandOld)
    {
        return textCommandOld.LastUsed.HasValue && textCommandOld.LastUsed > DateTime.Now.AddSeconds(-textCommandOld.TimeoutInSeconds);
    }

    private void UpdateCommandUsageInfo(TextCommandOld textCommandOld)
    {
        textCommandOld.UseCount++;
        textCommandOld.LastUsed = DateTime.Now;
        this.repository.Write(textCommandOld);
    }

    private static bool CommandMatches(TextCommandOld textCommandOld, IChatCommand command)
    {
        return textCommandOld.Commands.Any(c => c.Equals(command.CommandText, StringComparison.CurrentCultureIgnoreCase));
    }
}