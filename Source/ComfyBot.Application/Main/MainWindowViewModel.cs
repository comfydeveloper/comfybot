namespace ComfyBot.Application.Main;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class MainWindowViewModel
{
    public MainWindowViewModel(TabsViewModel tabs)
    {
        Tabs = tabs;
    }

    public TabsViewModel Tabs { get; }
}