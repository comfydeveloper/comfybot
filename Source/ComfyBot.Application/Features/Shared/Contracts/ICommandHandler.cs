using ComfyBot.Application.Patterns.Outcomes;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Shared.Contracts;

public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command);
}

public interface ICommandHandler<in TCommand>
{
    Task<Outcome> Handle(TCommand command);
}