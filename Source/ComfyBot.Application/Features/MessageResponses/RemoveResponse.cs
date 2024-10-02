using ComfyBot.Application.Features.Shared.Contracts;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComfyBot.Application.Features.MessageResponses;

public sealed class RemoveResponse
{
    public record Command(Guid Id);

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IQueryableRepository repository;

        public Handler(IQueryableRepository repository)
        {
            this.repository = repository;
        }

        public async Task Handle(Command command)
        {
            MessageResponse messageResponse = this.repository.Query<MessageResponse>().First(x => x.Id == command.Id);
            
            this.repository.Remove(messageResponse);

            await this.repository.SaveChanges();
        }
    }
}