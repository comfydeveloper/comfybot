using System.Collections.Generic;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Commands;

public class TextCommandHandler : CommandHandler
{
    private readonly IRepository<TextCommandOld> repository;
    private readonly ITextCommandReplyLoader replyLoader;

    public TextCommandHandler(IRepository<TextCommandOld> repository, ITextCommandReplyLoader replyLoader)
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
        IEnumerable<TextCommandOld> textCommands = this.repository.GetAll();

        foreach (TextCommandOld textCommand in textCommands)
        {
            if (this.replyLoader.TryGetReply(textCommand, command, out string reply))
            {
                this.SendMessage(client, reply);
                return;
            }
        }
    }
}