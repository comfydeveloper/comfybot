using ComfyBot.Application.Features.Shared;
using ComfyBot.Application.Features.Shared.Contracts;
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

        public async Task<Result> Handle(Query query)
        {
            List<TextCommand> textCommands = await this.repository.Query<TextCommand>().ToListAsync();

            return new Result
            {
                Entries = textCommands.Select(tc => new TextCommandEntry(
                    tc.Id,
                    tc.TimeoutInSeconds,
                    tc.Commands.ToArray(),
                    tc.Replies.ToArray())).ToList()
            };
        }
    }
}