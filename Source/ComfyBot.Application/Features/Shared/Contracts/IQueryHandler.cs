using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Shared.Contracts;

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> Handle(TQuery query);
}

public interface IQueryHandler<TResult>
{
    Task<TResult> Handle();
}