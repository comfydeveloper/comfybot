namespace ComfyBot.Bot.ChatBot.Commands
{
    using System;

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
                reply = textCommand.Replies.GetRandom();
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
            return string.Equals(command.CommandText, textCommand.Command, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}