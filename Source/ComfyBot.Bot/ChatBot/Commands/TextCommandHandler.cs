using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System.Linq;
using TwitchLib.Client.Interfaces;
using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Options;

namespace ComfyBot.Bot.ChatBot.Commands;

public class TextCommandHandler : CommandHandler
{
    private readonly IQueryableRepository repository;
    private readonly ITextCommandReplyLoader replyLoader;

    public TextCommandHandler(IQueryableRepository repository, ITextCommandReplyLoader replyLoader, IOptions<BotSettings> settings) : base(settings)
    {
        this.repository = repository;
        this.replyLoader = replyLoader;
    }

    protected override bool CanHandle(IChatCommand command)
    {
        return true;
    }

    protected override void HandleInternal(ITwitchClient client, IChatCommand chatCommand)
    {
        IQueryable<TextCommand> textCommands = this.repository.Query<TextCommand>();

        foreach (TextCommand textCommand in textCommands)
        {
            if (this.replyLoader.TryGetReply(textCommand, chatCommand, out string reply))
            {
                textCommand.UpdateLastUsage();
                this.repository.SaveChanges();

                this.SendMessage(client, reply);
                return;
            }
        }
    }
}