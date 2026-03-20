using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.TextCommands;

public sealed class UpdateCommand
{
    public record Command(Guid Id, int TimeoutInSeconds, string[] Commands, string[] Replies);

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IQueryableRepository repository;

        public Handler(IQueryableRepository repository)
        {
            this.repository = repository;
        }

        public async Task Handle(Command command)
        {
            TextCommand textCommand = this.repository.Query<TextCommand>().First(x => x.Id == command.Id);

            textCommand.TimeoutInSeconds = command.TimeoutInSeconds;
            textCommand.Commands = new List<string>(command.Commands);
            textCommand.Replies = new List<string>(command.Replies);

            await this.repository.SaveChanges();
        }
    }
}