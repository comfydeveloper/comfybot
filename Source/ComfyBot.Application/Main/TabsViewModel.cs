namespace ComfyBot.Application.Main
{
    using ComfyBot.Application.Shoutouts;

    public class TabsViewModel
    {
        public TabsViewModel(ShoutoutTabViewModel shoutouts)
        {
            this.Shoutouts = shoutouts;
        }

        public ShoutoutTabViewModel Shoutouts { get; }
    }
}