using System.Threading.Tasks;
using ComfyBot.Gateway.Contracts.Models;

namespace ComfyBot.Bot.ChatBot.Commands;

public interface ICommandHandler
{
    Task Handle(IChatCommand command);
}