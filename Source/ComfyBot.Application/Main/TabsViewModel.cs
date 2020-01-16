namespace ComfyBot.Application.Main
{
    using ComfyBot.Application.Responses;
    using ComfyBot.Application.Shoutouts;

    public class TabsViewModel
    {
        public TabsViewModel(ShoutoutTabViewModel shoutouts,
                             ResponseTabViewModel responses)
        {
            this.Shoutouts = shoutouts;
            this.Responses = responses;
        }

        public ResponseTabViewModel Responses { get; }

        public ShoutoutTabViewModel Shoutouts { get; }
    }
}