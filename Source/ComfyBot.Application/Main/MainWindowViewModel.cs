namespace ComfyBot.Application.Main
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel(TabsViewModel tabs)
        {
            this.Tabs = tabs;
        }

        public TabsViewModel Tabs { get; }
    }
}