using ComfyBot.Application.Features.Shared;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Variables;

public sealed class GetVariables
{
    public record Query;

    public record VariableEntry(
        Guid Id,
        string Name,
        string Value);

    public class Result : ListDto<VariableEntry>;

    internal class Handler : IQueryHandler<Query, Result>
    {
        private readonly IQueryableRepository repository;

        public Handler(IQueryableRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result> Handle(Query query)
        {
            List<VariableEntry> variables = await this.repository.Query<Variable>()
                .Select(x => new VariableEntry(x.Id, x.Name, x.Value)).ToListAsync();

            return new Result
            {
                Entries = variables
            };
        }
    }
}