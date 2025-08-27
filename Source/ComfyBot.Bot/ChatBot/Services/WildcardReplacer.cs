using System.Collections.Generic;
using ComfyBot.Bot.ChatBot.Services.Strategies;

namespace ComfyBot.Bot.ChatBot.Services;

public class WildcardReplacer : IWildcardReplacer
{
    private readonly IEnumerable<IReplacementStrategy> replacementStrategies = [];

    public WildcardReplacer(IEnumerable<IReplacementStrategy> replacementStrategies)
    {
        this.replacementStrategies = replacementStrategies;
    }

    public string Replace(string original)
    {
        return this.Replace(original, new WildcardReplacerOptions());
    }

    public string Replace(string original, WildcardReplacerOptions options)
    {
        string result = original;
        
        foreach (IReplacementStrategy replacementStrategy in this.replacementStrategies)
        {
            result = replacementStrategy.Replace(result, options);
        }

        return result;
    }
}