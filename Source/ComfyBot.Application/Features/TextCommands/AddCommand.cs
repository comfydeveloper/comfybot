using AutoMapper;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.TextCommands;

public sealed class AddCommand
{
    public record Command(Guid Id);

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Command, TextCommand>(MemberList.Source);
        }
    }

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IQueryableRepository repository;
        private readonly IMapper mapper;

        public Handler(IQueryableRepository repository,
                       IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task Handle(Command command)
        {
            TextCommand newCommand = this.mapper.Map<TextCommand>(command);

            this.repository.Add(newCommand);
            await this.repository.SaveChanges();
        }
    }
}