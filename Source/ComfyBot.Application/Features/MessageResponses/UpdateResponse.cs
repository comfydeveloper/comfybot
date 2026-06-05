using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Patterns.Outcomes;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.MessageResponses;

public sealed class UpdateResponse
{
    public record Command(Guid Id, int TimeoutInSeconds, bool AlwaysReply, int Priority, string[] Users, string[] ExactKeywords, string[] LooseKeywords, string[] AllKeywords, string[] Replies);

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
                MessageResponse messageResponse = this.repository.Query<MessageResponse>().FirstOrDefault(x => x.Id == command.Id);

                if (messageResponse is null)
                {
                    return Outcome.Failure(new NotFoundError("MessageResponse", command.Id.ToString()));
                }

                messageResponse.TimeoutInSeconds = command.TimeoutInSeconds;
                messageResponse.AlwaysReply = command.AlwaysReply;
                messageResponse.Priority = command.Priority;
                messageResponse.Users = new List<string>(command.Users);
                messageResponse.ExactKeywords = new List<string>(command.ExactKeywords);
                messageResponse.LooseKeywords = new List<string>(command.LooseKeywords);
                messageResponse.AllKeywords = new List<string>(command.AllKeywords);
                messageResponse.Replies = new List<string>(command.Replies);

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