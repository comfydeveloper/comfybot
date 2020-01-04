namespace ComfyBot.Application.Bot.Commands
{
    using System.Linq;

    using global::ComfyBot.Application.Bot.Wrappers;
    using global::ComfyBot.Application.Data;
    using global::ComfyBot.Application.Data.Models;
    using global::ComfyBot.Application.Data.Wrappers;

    using Microsoft.Extensions.Configuration;

    using TwitchLib.Client.Interfaces;

    public class ShoutoutCommandHandler : CommandHandler
    {
        private readonly IRepository<Shoutout> repository;

        public ShoutoutCommandHandler(IConfiguration configuration,
                                      IRepository<Shoutout> repository)
            : base(configuration)
        {
            this.repository = repository;
        }

        protected override bool CanHandle(IChatCommand command)
        {
            return command.CommandText.ToLower().StartsWith("so");
        }

        protected override void HandleInternal(ITwitchClient client, IChatCommand command)
        {
            string[] arguments = command.ArgumentsAsString.Split(' ', 3);

            if (arguments[0].ToLower() == "set" && arguments.Length == 3)
            {
                if (command.ChatMessage.IsModerator || command.ChatMessage.IsBroadcaster)
                {
                    Shoutout model = new Shoutout
                                     {
                                         Id = arguments[1],
                                         ShoutoutText = arguments[2]
                                     };
                    this.repository.AddOrUpdate(model);
                }
            }
            else
            {
                Shoutout model = this.repository.Get(arguments[0]);

                if (model != null)
                {
                    this.SendMessage(client, $@"▬▬☆✼★▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬★✼☆▬▬
                                                {model.ShoutoutText}      
                                                ▬▬☆✼★━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━★✼☆▬▬");
                }
            }
        }
    }
}