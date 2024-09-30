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

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<MessageResponse, MessageResponseEntry>(MemberList.Destination);
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
            List<MessageResponse> messageResponses = await this.repository.Query<MessageResponse>().ToListAsync();

            return new Result
            {
                Entries = messageResponses.Select(this.mapper.Map<MessageResponseEntry>).ToList()
            };
        }
    }
}