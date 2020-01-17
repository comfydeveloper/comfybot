namespace ComfyBot.Application.Main
{
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Application.Configuration;
    using ComfyBot.Application.Responses;
    using ComfyBot.Application.Shoutouts;
    using ComfyBot.Application.TextCommands;

    [ExcludeFromCodeCoverage]
    public class TabsViewModel
    {
        public TabsViewModel(ShoutoutTabViewModel shoutouts,
                             ResponseTabViewModel responses,
                             ConfigurationTabViewModel configuration,
                             TextCommandsTabViewModel textCommands)
        {
            this.Shoutouts = shoutouts;
            this.Responses = responses;
            this.Configuration = configuration;
            this.TextCommands = textCommands;
        }

        public ConfigurationTabViewModel Configuration { get; }

        public TextCommandsTabViewModel TextCommands { get; }

        public ResponseTabViewModel Responses { get; }

        public ShoutoutTabViewModel Shoutouts { get; }
    }
}