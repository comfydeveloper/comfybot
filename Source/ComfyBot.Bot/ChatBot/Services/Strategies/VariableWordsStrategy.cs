using System.Text.RegularExpressions;
using System;
using ComfyBot.Bot.Extensions;

namespace ComfyBot.Bot.ChatBot.Services.Strategies;

public class VariableWordsStrategy : IReplacementStrategy
{
    public string Replace(string text, WildcardReplacerOptions options)
    {
        MatchCollection matches = Regex.Matches(text, @"\[w:(.*?)\]");
        Random random = new Random();

        foreach (Match match in matches)
        {
            string variableWordsPart = match.Groups[1].Value;
            string[] words = variableWordsPart.Split(',');

            int randomIndex = random.Next(0, words.Length);
            string randomWord = words[randomIndex];

            text = text.ReplaceFirst($"{match.Groups[0].Value}", randomWord);
        }
        return text;
    }
}