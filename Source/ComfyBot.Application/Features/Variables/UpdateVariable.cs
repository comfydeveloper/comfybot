using AutoMapper;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.Variables;

public sealed class UpdateVariable
{
    public record Command(Guid Id, string Name, string Value);

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Command, Variable>(MemberList.Source)
                .ForMember(x => x.Id, o => o.Ignore());
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
            Variable textCommand = this.repository.Query<Variable>().First(x => x.Id == command.Id);

            this.mapper.Map(command, textCommand);

            await this.repository.SaveChanges();
        }
    }
}