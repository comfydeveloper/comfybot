using ComfyBot.Bot.ChatBot.Services;
using System.Linq;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;

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

    public void Handle(IChatMessage message)
    {
        if (IsCommand(message))
        {
            return;
        }

        MessageResponse[] messageResponses = this.repository.Query<MessageResponse>().OrderBy(r => r.Priority).ToArray();

        foreach (MessageResponse messageResponse in messageResponses)
        {
            if (this.responseLoader.TryGetResponse(messageResponse, message, out string response))
            {
                messageResponse.UpdateLastUsage();
                this.repository.SaveChanges();
                this.messageSender.Send(response);
                return;
            }
        }
    }

    private static bool IsCommand(IChatMessage message)
    {
        return message.Text.StartsWith('!');
    }
}