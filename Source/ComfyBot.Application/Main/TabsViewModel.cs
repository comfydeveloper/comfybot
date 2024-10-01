using System.Diagnostics.CodeAnalysis;
using ComfyBot.Application.Responses;
using ComfyBot.Application.TextCommands;

namespace ComfyBot.Application.Main;

[ExcludeFromCodeCoverage]
public class TabsViewModel
{
    public TabsViewModel(ResponseTabViewModel responses,
        TextCommandsTabViewModel textCommands)
    {
        this.Responses = responses;
        this.TextCommands = textCommands;
    }

    public TextCommandsTabViewModel TextCommands { get; }

    public ResponseTabViewModel Responses { get; }
}