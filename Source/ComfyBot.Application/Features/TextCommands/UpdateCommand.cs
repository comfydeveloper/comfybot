using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Patterns.Outcomes;
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

        public async Task<Outcome> Handle(Command command)
        {
            try
            {
                TextCommand textCommand = this.repository.Query<TextCommand>().FirstOrDefault(x => x.Id == command.Id);

                if (textCommand is null)
                {
                    return Outcome.Failure(new NotFoundError("TextCommand", command.Id.ToString()));
                }

                textCommand.TimeoutInSeconds = command.TimeoutInSeconds;
                textCommand.Commands = new List<string>(command.Commands);
                textCommand.Replies = new List<string>(command.Replies);

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