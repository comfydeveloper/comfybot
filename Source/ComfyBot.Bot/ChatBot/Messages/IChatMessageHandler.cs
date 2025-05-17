using ComfyBot.Bot.ChatBot.Wrappers;
using System.Threading.Tasks;

namespace ComfyBot.Bot.ChatBot.Messages;

public interface IChatMessageHandler
{
    Task Handle(IChatMessage message);
}