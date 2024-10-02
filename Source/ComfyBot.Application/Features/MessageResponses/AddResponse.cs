using AutoMapper;
using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.MessageResponses;

public sealed class AddResponse
{
    public record Command(Guid Id);

    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<Command, MessageResponse>(MemberList.Source);
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
            MessageResponse messageResponse = this.mapper.Map<MessageResponse>(command);

            this.repository.Add(messageResponse);
            await this.repository.SaveChanges();
        }
    }
}