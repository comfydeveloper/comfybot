using System.Text.RegularExpressions;
using System;
using ComfyBot.Bot.Extensions;

namespace ComfyBot.Bot.ChatBot.Services.Strategies;

public class NumberRangeStrategy : IReplacementStrategy
{
    public string Replace(string text, WildcardReplacerOptions options)
    {
        MatchCollection matches = Regex.Matches(text, @"\[n:(.*?)\]");
        Random random = new();

        foreach (Match match in matches)
        {
            string variableWordsPart = match.Groups[1].Value;
            string[] numbers = variableWordsPart.Split('-');

            if (int.TryParse(numbers[0], out int minimum) && int.TryParse(numbers[1], out int maximum))
            {
                int randomNumber = random.Next(minimum, maximum + 1);
                text = text.ReplaceFirst($"{match.Groups[0].Value}", randomNumber.ToString());
            }
        }
        return text;
    }
}