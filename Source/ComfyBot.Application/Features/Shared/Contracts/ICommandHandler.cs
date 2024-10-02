using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Shared.Contracts;

public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command);
}

public interface ICommandHandler<in TResult>
{
    Task Handle(TResult command);
}