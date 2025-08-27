using ComfyBot.Application.Features.Shared.Contracts;
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

        public async Task Handle(Command command)
        {
            Variable variable = this.repository.Query<Variable>().First(x => x.Id == command.Id);

            this.repository.Remove(variable);

            await this.repository.SaveChanges();
        }
    }
}