namespace ComfyBot.Application.Main;

using System.Diagnostics.CodeAnalysis;

using Configuration;
using Responses;
using TextCommands;

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