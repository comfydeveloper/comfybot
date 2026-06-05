using ComfyBot.Application.Patterns.Outcomes;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Shared.Contracts;

public interface IQueryHandler<in TQuery, TResult>
{
    Task<Outcome<TResult>> Handle(TQuery query);
}

public interface IQueryHandler<TResult>
{
    Task<Outcome<TResult>> Handle();
}