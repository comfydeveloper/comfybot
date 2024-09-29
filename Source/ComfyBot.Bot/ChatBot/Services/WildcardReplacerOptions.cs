using System;

namespace ComfyBot.Bot.ChatBot.Services;

public class WildcardReplacerOptions
{
    public string[] Parameters { get; set; } = Array.Empty<string>();

    public string UserName { get; set; }
}