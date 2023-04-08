using System.Diagnostics.CodeAnalysis;
using ComfyBot.Application.Configuration;
using ComfyBot.Application.Responses;
using ComfyBot.Application.TextCommands;

namespace ComfyBot.Application.Main;

[ExcludeFromCodeCoverage]
public class TabsViewModel
{
    public TabsViewModel(ResponseTabViewModel responses,
        ConfigurationTabViewModel configuration,
        TextCommandsTabViewModel textCommands)
    {
        Responses = responses;
        Configuration = configuration;
        TextCommands = textCommands;
    }

    public ConfigurationTabViewModel Configuration { get; }

    public TextCommandsTabViewModel TextCommands { get; }

    public ResponseTabViewModel Responses { get; }
}