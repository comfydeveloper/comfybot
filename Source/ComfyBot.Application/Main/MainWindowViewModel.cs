using System.Diagnostics.CodeAnalysis;

namespace ComfyBot.Application.Main;

[ExcludeFromCodeCoverage]
public class MainWindowViewModel
{
    public MainWindowViewModel(TabsViewModel tabs)
    {
        Tabs = tabs;
    }

    public TabsViewModel Tabs { get; }
}