using ComfyBot.Bot.ChatBot.Messages.Extensions;
using ComfyBot.Bot.ChatBot.Services;
using System.Linq;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace ComfyBot.Bot.ChatBot.Messages;

public class ChatMessageResponseHandler : IChatMessageHandler
{
    private readonly IQueryableRepository repository;
    private readonly IMessageResponseLoader responseLoader;
    private readonly IMessageSender messageSender;

    public ChatMessageResponseHandler(IQueryableRepository repository,
        IMessageResponseLoader responseLoader,
        IMessageSender messageSender)
    {
        this.repository = repository;
        this.responseLoader = responseLoader;
        this.messageSender = messageSender;
    }

    public async Task Handle(IChatMessage message)
    {
        if (message.IsCommand())
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