using ComfyBot.Application.Shared;
using System.Collections.Generic;

namespace ComfyBot.Application.Responses;

public class MessageResponseModel
{
    public string Id { get; set; } = string.Empty;

    public List<TextModel> Users { get; set; } = [];

    public List<TextModel> ExactKeywords { get; set; } = [];

    public List<TextModel> LooseKeywords { get; set; } = [];

    public List<TextModel> AllKeywords { get; set; } = [];

    public List<TextModel> Replies { get; set; } = [];

    public int TimeoutInSeconds { get; set; } = 60;

    public bool ReplyAlways { get; set; }

    public int Priority { get; set; }
}
