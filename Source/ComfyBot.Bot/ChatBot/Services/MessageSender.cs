using ComfyBot.Bot.Scaffolding;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Interfaces;

namespace ComfyBot.Bot.ChatBot.Services;

public class MessageSender : IMessageSender
{
    private readonly ITwitchClient client;
    private readonly BotSettings settings;

    public MessageSender(ITwitchClient client, IOptions<BotSettings> settings)
    {
        this.client = client;
        this.settings = settings.Value;
    }

    public void Send(string message)
    {
        this.client.SendMessage(this.settings.Channel, message);
    }
}