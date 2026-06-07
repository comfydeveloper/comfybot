using ComfyBot.Application.Features.Shared;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Application.Patterns.Outcomes;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.MessageResponses;

public sealed class GetResponses
{
    public record Query;

    public record MessageResponseEntry(
        Guid Id,
        int TimeoutInSeconds,
        bool AlwaysReply,
        int Priority,
        string[] Users,
        string[] ExactKeywords,
        string[] LooseKeywords,
        string[] AllKeywords,
        string[] Replies);

    public class Result : ListDto<MessageResponseEntry>;

    internal class Handler : IQueryHandler<Query, Result>
    {
        private readonly IQueryableRepository repository;

        public Handler(IQueryableRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Outcome<Result>> Handle(Query query)
        {
            try
            {
                List<MessageResponse> messageResponses = await this.repository.Query<MessageResponse>().ToListAsync();

                Result result = new()
                {
                    Entries = messageResponses.Select(mr => new MessageResponseEntry(
                        mr.Id,
                        mr.TimeoutInSeconds,
                        mr.AlwaysReply,
                        mr.Priority,
                        mr.Users.ToArray(),
                        mr.ExactKeywords.ToArray(),
                        mr.LooseKeywords.ToArray(),
                        mr.AllKeywords.ToArray(),
                        mr.Replies.ToArray())).ToList()
                };

                return Outcome<Result>.Success(result);
            }
            catch (Exception ex)
            {
                return Outcome<Result>.Failure(new DatabaseError(ex.Message));
            }
        }
    }
}