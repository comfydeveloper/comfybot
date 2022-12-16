namespace ComfyBot.Bot.ChatBot.Commands;

using System;
using System.Linq;
using Services;
using Wrappers;
using Extensions;
using Data.Models;
using Data.Repositories;

public class TextCommandReplyLoader : ITextCommandReplyLoader
{
    private readonly IRepository<TextCommand> repository;
    private readonly IWildcardReplacer wildcardReplacer;

    public TextCommandReplyLoader(IRepository<TextCommand> repository, IWildcardReplacer wildcardReplacer)
    {
        this.repository = repository;
        this.wildcardReplacer = wildcardReplacer;
    }

    public bool TryGetReply(TextCommand textCommand, IChatCommand command, out string reply)
    {
        if (!HasOngoingTimeout(textCommand) && CommandMatches(textCommand, command))
        {
            UpdateCommandUsageInfo(textCommand);

            if (command.ArgumentsAsList.Any())
            {
                string[] repliesWithParameters = textCommand.Replies.Where(r => r.Contains("{{parameters}}")
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
                    reply = wildcardReplacer.Replace(reply);
                    return true;
                }
            }
            reply = textCommand.Replies.Where(r => !r.Contains("{{parameter")).GetRandom();
            reply = reply.Replace("{{user}}", command.ChatMessage.UserName);
            reply = wildcardReplacer.Replace(reply);
            return true;
        }
        reply = null;
        return false;
    }

    private bool HasOngoingTimeout(TextCommand textCommand)
    {
        return textCommand.LastUsed.HasValue && textCommand.LastUsed > DateTime.Now.AddSeconds(-textCommand.TimeoutInSeconds);
    }

    private void UpdateCommandUsageInfo(TextCommand textCommand)
    {
        textCommand.UseCount++;
        textCommand.LastUsed = DateTime.Now;
        repository.AddOrUpdate(textCommand);
    }

    private static bool CommandMatches(TextCommand textCommand, IChatCommand command)
    {
        return textCommand.Commands.Any(c => c.Equals(command.CommandText, StringComparison.CurrentCultureIgnoreCase));
    }
}