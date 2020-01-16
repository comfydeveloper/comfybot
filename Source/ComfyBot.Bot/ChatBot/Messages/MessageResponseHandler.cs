namespace ComfyBot.Bot.ChatBot.Messages
{
    using System.Collections.Generic;

    using ComfyBot.Bot.ChatBot.Wrappers;
    using ComfyBot.Data.Models;
    using ComfyBot.Data.Repositories;
    using ComfyBot.Settings;

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
            IEnumerable<MessageResponse> messageResponses = this.repository.GetAll();

            foreach (MessageResponse messageResponse in messageResponses)
            {
                if (this.responseLoader.TryGetResponse(messageResponse, message, out string response))
                {
                    client.SendMessage(ApplicationSettings.Default.Channel, response);
                    return;
                }
            }
        }
    }
}