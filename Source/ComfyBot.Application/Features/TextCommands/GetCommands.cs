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

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<TextCommand, TextCommandEntry>(MemberList.Destination);
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
            List<TextCommand> textCommands = await this.repository.Query<TextCommand>().ToListAsync();

            return new Result
            {
                Entries = textCommands.Select(this.mapper.Map<TextCommandEntry>).ToList()
            };
        }
    }
}