using ComfyBot.Bot.ChatBot.Messages.Extensions;
using ComfyBot.Bot.ChatBot.Services;
using System.Linq;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Scaffolding;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ComfyBot.Bot.ChatBot.Messages;

public class ChatMessageResponseHandler : IChatMessageHandler
{
    private readonly IQueryableRepository repository;
    private readonly IMessageResponseLoader responseLoader;
    private readonly IMessageSender messageSender;
    private readonly BotSettings settings;

    public ChatMessageResponseHandler(IQueryableRepository repository,
        IMessageResponseLoader responseLoader,
        IMessageSender messageSender,
        IOptions<BotSettings> options)
    {
        this.repository = repository;
        this.responseLoader = responseLoader;
        this.messageSender = messageSender;
        this.settings = options.Value;
    }

    public async Task Handle(IChatMessage message)
    {
        if (message.IsCommand() || message.SentBy(this.settings.IgnoredUsers))
        {
            return;
        }

        MessageResponse[] messageResponses = this.repository.Query<MessageResponse>().OrderBy(r => r.Priority).ToArray();

        foreach (MessageResponse messageResponse in messageResponses)
        {
            if (this.responseLoader.TryGetResponse(messageResponse, message, out string response))
            {
                messageResponse.UpdateLastUsage();
                await this.repository.SaveChanges();

                int waitTime = Random.Shared.Next(2500, 11000);
                await Task.Delay(waitTime);

                this.messageSender.Send(response);
                return;
            }
        }
    }
}