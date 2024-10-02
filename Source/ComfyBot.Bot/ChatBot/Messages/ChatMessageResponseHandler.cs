using System.Linq;
using ComfyBot.Bot.ChatBot.Wrappers;
using ComfyBot.Bot.Scaffolding;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Messages;

public class ChatMessageResponseHandler : IChatMessageHandler
{
    private readonly IQueryableRepository repository;
    private readonly IMessageResponseLoader responseLoader;
    private readonly BotSettings settings;

    public ChatMessageResponseHandler(IQueryableRepository repository,
        IMessageResponseLoader responseLoader,
        IOptions<BotSettings> settings)
    {
        this.repository = repository;
        this.responseLoader = responseLoader;
        this.settings = settings.Value;
    }

    // TODO [Shae] Move client creation out of these calls and use DI instead -> Singleton
    public void Handle(ITwitchClient client, IChatMessage message)
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
                client.SendMessage(this.settings.Channel, response);
                return;
            }
        }
    }

    private static bool IsCommand(IChatMessage message)
    {
        return message.Text.StartsWith('!');
    }
}