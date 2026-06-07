using ComfyBot.Bot.ChatBot.Services;
using ComfyBot.Gateway.Contracts.Models;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

    protected override async Task HandleInternal(IChatCommand chatCommand)
    {
        TextCommand[] textCommands = await this.repository.Query<TextCommand>().ToArrayAsync();

        foreach (TextCommand textCommand in textCommands)
        {
            if (this.replyLoader.TryGetReply(textCommand, chatCommand, out string reply))
            {
                textCommand.UpdateLastUsage();
                await this.repository.SaveChanges();

                this.messageSender.Send(reply);
                return;
            }
        }
    }
}