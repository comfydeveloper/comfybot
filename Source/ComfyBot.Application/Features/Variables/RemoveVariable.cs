using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Patterns.Outcomes;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Variables;

public sealed class RemoveVariable
{
    public record Command(Guid Id);

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IQueryableRepository repository;

        public Handler(IQueryableRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Outcome> Handle(Command command)
        {
            try
            {
                Variable variable = this.repository.Query<Variable>().FirstOrDefault(x => x.Id == command.Id);

                if (variable is null)
                {
                    return Outcome.Failure(new NotFoundError("Variable", command.Id.ToString()));
                }

                this.repository.Remove(variable);
                await this.repository.SaveChanges();

                return Outcome.Success();
            }
            catch (Exception ex)
            {
                return Outcome.Failure(new DatabaseError(ex.Message));
            }
        }
    }
}