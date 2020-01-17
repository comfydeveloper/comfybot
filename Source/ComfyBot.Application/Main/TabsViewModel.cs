namespace ComfyBot.Application.Main
{
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Application.Configuration;
    using ComfyBot.Application.Responses;
    using ComfyBot.Application.Shoutouts;

    [ExcludeFromCodeCoverage]
    public class TabsViewModel
    {
        public TabsViewModel(ShoutoutTabViewModel shoutouts,
                             ResponseTabViewModel responses,
                             ConfigurationTabViewModel configuration)
        {
            this.Shoutouts = shoutouts;
            this.Responses = responses;
            this.Configuration = configuration;
        }

        public ConfigurationTabViewModel Configuration { get; }

        public ResponseTabViewModel Responses { get; }

        public ShoutoutTabViewModel Shoutouts { get; }
    }
}