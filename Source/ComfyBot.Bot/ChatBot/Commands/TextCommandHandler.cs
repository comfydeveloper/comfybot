namespace ComfyBot.Bot.ChatBot.Commands
{
    using System.Collections.Generic;

    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;
    using ComfyBot.Settings;

    using TwitchLib.Client.Interfaces;

    public class TextCommandHandler : CommandHandler
    {
        private readonly IRepository<TextCommand> repository;
        private readonly ITextCommandReplyLoader replyLoader;

        public TextCommandHandler(IRepository<TextCommand> repository, ITextCommandReplyLoader replyLoader)
        {
            this.repository = repository;
            this.replyLoader = replyLoader;
        }

        protected override bool CanHandle(IChatCommand command)
        {
            return true;
        }

        protected override void HandleInternal(ITwitchClient client, IChatCommand command)
        {
            IEnumerable<TextCommand> textCommands = this.repository.GetAll();

            foreach (TextCommand textCommand in textCommands)
            {
                if (this.replyLoader.TryGetReply(textCommand, command, out string reply))
                {
                    SendMessage(client, reply);
                    return;
                }
            }
        }
    }
}