using AutoMapper;
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

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Variable, VariableEntry>(MemberList.Destination);
        }
    }

    internal class Handler : IQueryHandler<Query, Result>
    {
        private readonly IQueryableRepository repository;
        private readonly IMapper mapper;

        public Handler(IQueryableRepository repository,
                       IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(Query query)
        {
            List<Variable> variables = await this.repository.Query<Variable>().ToListAsync();

            return new Result
            {
                Entries = variables.Select(this.mapper.Map<VariableEntry>).ToList()
            };
        }
    }
}