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

namespace ComfyBot.Application.Features.TextCommands;

public sealed class GetCommands
{
    public record Query;

    public record TextCommandEntry(
        Guid Id,
        int TimeoutInSeconds,
        string[] Commands,
        string[] Replies);

    public class Result : ListDto<TextCommandEntry>;

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
                List<TextCommand> textCommands = await this.repository.Query<TextCommand>().ToListAsync();

                Result result = new()
                {
                    Entries = textCommands.Select(tc => new TextCommandEntry(
                        tc.Id,
                        tc.TimeoutInSeconds,
                        tc.Commands.ToArray(),
                        tc.Replies.ToArray())).ToList()
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