using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System.Linq;

namespace ComfyBot.Bot.ChatBot.Commands;

public class TextCommandHandler : CommandHandler
{
    private readonly IQueryableRepository repository;
    private readonly ITextCommandReplyLoader replyLoader;
    private readonly IMessageSender messageSender;

    public TextCommandHandler(IQueryableRepository repository,
                              ITextCommandReplyLoader replyLoader,
                              IMessageSender messageSender)
    {
        this.repository = repository;
        this.replyLoader = replyLoader;
        this.messageSender = messageSender;
    }

    protected override bool CanHandle(IChatCommand command)
    {
        return true;
    }

    protected override void HandleInternal(IChatCommand chatCommand)
    {
        IQueryable<TextCommand> textCommands = this.repository.Query<TextCommand>();

        foreach (TextCommand textCommand in textCommands)
        {
            if (this.replyLoader.TryGetReply(textCommand, chatCommand, out string reply))
            {
                textCommand.UpdateLastUsage();
                this.repository.SaveChanges();

                this.messageSender.Send(reply);
                return;
            }
        }
    }
}