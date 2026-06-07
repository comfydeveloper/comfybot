using ComfyBot.Application.Shared;
using System.Collections.Generic;

namespace ComfyBot.Application.TextCommands;

public class TextCommandModel
{
    public string Id { get; set; } = string.Empty;

    public List<TextModel> Replies { get; set; } = [];

    public List<TextModel> Commands { get; set; } = [];

    public int Timeout { get; set; } = 30;
}