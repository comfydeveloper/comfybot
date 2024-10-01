using AutoMapper;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.TextCommands;

public sealed class UpdateCommand
{
    public record Command(Guid Id, int TimeoutInSeconds, string[] Commands, string[] Replies);

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Command, TextCommand>(MemberList.Source)
                .ForMember(x => x.Id, o => o.Ignore())
                .ForMember(x => x.LastUsedAt, o => o.Ignore())
                .ForMember(x => x.UseCount, o => o.Ignore());
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
            TextCommand textCommand = this.repository.Query<TextCommand>().First(x => x.Id == command.Id);

            this.mapper.Map(command, textCommand);

            await this.repository.SaveChanges();
        }
    }
}