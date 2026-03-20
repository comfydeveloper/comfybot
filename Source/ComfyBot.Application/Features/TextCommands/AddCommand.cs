using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.TextCommands;

public sealed class AddCommand
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
            TextCommand newCommand = new()
            {
                Id = command.Id,
                CreatedAt = DateTime.Now,
                Replies = [],
                Commands = [],
                LastUsedAt = null,
                UseCount = 0,
                TimeoutInSeconds = 0
            };

            this.repository.Add(newCommand);
            await this.repository.SaveChanges();
        }
    }
}