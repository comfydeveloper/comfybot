using System.Threading.Tasks;
using ComfyBot.Gateway.Contracts.Models;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IChatMessageHandler
{
    Task Handle(IChatMessage message);
}