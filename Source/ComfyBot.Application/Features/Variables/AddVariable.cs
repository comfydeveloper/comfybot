using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Variables;

public sealed class AddVariable
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
            Variable newVariable = new()
            {
                Id = command.Id
            };

            this.repository.Add(newVariable);
            await this.repository.SaveChanges();
        }
    }
}