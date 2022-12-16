﻿namespace ComfyBot.Bot.ChatBot.Messages;

using System.Collections.Generic;
using System.Linq;

using Wrappers;
using Data.Models;
using Data.Repositories;
using Settings;

using TwitchLib.Client.Interfaces;

public class MessageResponseHandler : IMessageHandler
{
    private readonly IRepository<MessageResponse> repository;
    private readonly IMessageResponseLoader responseLoader;

    public MessageResponseHandler(IRepository<MessageResponse> repository,
        IMessageResponseLoader responseLoader)
    {
        this.repository = repository;
        this.responseLoader = responseLoader;
    }

    public void Handle(ITwitchClient client, IChatMessage message)
    {
        if (IsCommand(message))
        {
            return;
        }

        IEnumerable<MessageResponse> messageResponses = repository.GetAll().OrderBy(r => r.Priority);

        foreach (MessageResponse messageResponse in messageResponses)
        {
            if (responseLoader.TryGetResponse(messageResponse, message, out string response))
            {
                client.SendMessage(ApplicationSettings.Default.Channel, response);
                return;
            }
        }
    }

    private static bool IsCommand(IChatMessage message)
    {
        return message.Text.StartsWith("!");
    }
}