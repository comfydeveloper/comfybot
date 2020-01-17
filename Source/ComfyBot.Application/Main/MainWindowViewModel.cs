namespace ComfyBot.Application.Main
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class MainWindowViewModel
    {
        public MainWindowViewModel(TabsViewModel tabs)
        {
            this.Tabs = tabs;
        }

        public TabsViewModel Tabs { get; }
    }
}