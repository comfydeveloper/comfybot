using System.Diagnostics.CodeAnalysis;
using ComfyBot.Application.Responses;
using ComfyBot.Application.TextCommands;
using ComfyBot.Application.Variables;

namespace ComfyBot.Application.Main;

[ExcludeFromCodeCoverage]
public class TabsViewModel
{
    public TabsViewModel(ResponseTabViewModel responses,
        TextCommandsTabViewModel textCommands,
        VariablesTabViewModel variables)
    {
        this.Responses = responses;
        this.TextCommands = textCommands;
        this.Variables = variables;
    }

    public TextCommandsTabViewModel TextCommands { get; }

    public ResponseTabViewModel Responses { get; }

    public VariablesTabViewModel Variables { get; set; }
}