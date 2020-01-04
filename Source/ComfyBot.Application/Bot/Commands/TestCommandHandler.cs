namespace ComfyBot.Application.Bot.Commands
{
    using global::ComfyBot.Application.Bot.Wrappers;

    using Microsoft.Extensions.Configuration;

    using TwitchLib.Client.Interfaces;

    public class TestCommandHandler : CommandHandler
    {
        public TestCommandHandler(IConfiguration configuration) : base(configuration)
        { }

        protected override bool CanHandle(IChatCommand command)
        {
            return command.CommandText.ToLower() == "test";
        }

        protected override void HandleInternal(ITwitchClient client, IChatCommand command)
        {
            this.SendMessage(client, "All working. You're good to go!");
        }
    }
}