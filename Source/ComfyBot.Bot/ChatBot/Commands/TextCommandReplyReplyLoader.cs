namespace ComfyBot.Bot.ChatBot.Commands
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

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
                this.UpdateCommandUsageInfo(textCommand);

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
                        reply = this.ReplaceVariableWords(reply);

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
                reply = this.ReplaceVariableWords(reply);
                return true;
            }
            reply = null;
            return false;
        }

        private string ReplaceVariableWords(string reply)
        {
            MatchCollection matches = Regex.Matches(reply, @"\[w:(.*?)\]");
            Random random = new Random();

            foreach (Match match in matches)
            {
                string variableWordsPart = match.Groups[1].Value;
                string[] words = variableWordsPart.Split(',');


                int randomNumber = random.Next(0, words.Length);
                string randomWord = words[randomNumber];

                reply = reply.Replace($"{match.Groups[0].Value}", randomWord);
            }
            return reply;
        }

        private bool HasOngoingTimeout(TextCommand textCommand)
        {
            return textCommand.LastUsed.HasValue && textCommand.LastUsed > DateTime.Now.AddSeconds(-textCommand.TimeoutInSeconds);
        }

        private void UpdateCommandUsageInfo(TextCommand textCommand)
        {
            textCommand.UseCount++;
            textCommand.LastUsed = DateTime.Now;
            this.repository.AddOrUpdate(textCommand);
        }

        private static bool CommandMatches(TextCommand textCommand, IChatCommand command)
        {
            return textCommand.Commands.Any(c => c.Equals(command.CommandText, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}