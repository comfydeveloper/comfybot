namespace ComfyBot.Bot.ChatBot.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Bot.Extensions;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;

    public class TextCommandReplyReplyLoader : ITextCommandReplyLoader
    {
        private readonly IRepository<TextCommand> repository;

        public TextCommandReplyReplyLoader(IRepository<TextCommand> repository)
        {
            this.repository = repository;
        }

        public bool TryGetReply(TextCommand textCommand, IChatCommand command, out string reply)
        {
            if (!HasOngoingTimeout(textCommand) && CommandMatches(textCommand, command))
            {
                this.UpdateLastUsageDate(textCommand);

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
                        return true;
                    }
                }
                reply = textCommand.Replies.Where(r => !r.Contains("{{parameter")).GetRandom();
                reply = reply.Replace("{{user}}", command.ChatMessage.UserName);
                return true;
            }
            reply = null;
            return false;
        }

        private bool HasOngoingTimeout(TextCommand textCommand)
        {
            return textCommand.LastUsed.HasValue && textCommand.LastUsed > DateTime.Now.AddSeconds(-textCommand.TimeoutInSeconds);
        }

        private void UpdateLastUsageDate(TextCommand textCommand)
        {
            textCommand.LastUsed = DateTime.Now;
            this.repository.AddOrUpdate(textCommand);
        }

        private static bool CommandMatches(TextCommand textCommand, IChatCommand command)
        {
            return textCommand.Commands.Any(c => c.Equals(command.CommandText, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}