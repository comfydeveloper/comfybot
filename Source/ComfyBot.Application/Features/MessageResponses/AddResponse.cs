using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Patterns.Outcomes;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.MessageResponses;

public sealed class AddResponse
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
                MessageResponse messageResponse = new()
                {
                    Id = command.Id,
                    CreatedAt = DateTime.UtcNow,
                    Users = [],
                    LooseKeywords = [],
                    AllKeywords = [],
                    ExactKeywords = [],
                    Replies = [],
                    LastUsedAt = null,
                    TimeoutInSeconds = 30,
                    UseCount = 0,
                    Priority = 0,
                    AlwaysReply = false
                };

                this.repository.Add(messageResponse);
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