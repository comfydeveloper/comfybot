using System;
using System.Text.RegularExpressions;
using ComfyBot.Bot.Extensions;

namespace ComfyBot.Bot.ChatBot.Services;

public class WildcardReplacer : IWildcardReplacer
{
    public string Replace(string original)
    {
        string result = ReplaceVariableWords(original);
        result = ReplaceNumberRange(result);

        return result;
    }

    private static string ReplaceNumberRange(string original)
    {
        MatchCollection matches = Regex.Matches(original, @"\[n:(.*?)\]");
        Random random = new Random();

        foreach (Match match in matches)
        {
            string variableWordsPart = match.Groups[1].Value;
            string[] numbers = variableWordsPart.Split('-');

            if (int.TryParse(numbers[0], out int minimum) && int.TryParse(numbers[1], out int maximum))
            {
                int randomNumber = random.Next(minimum, maximum + 1);
                original = original.ReplaceFirst($"{match.Groups[0].Value}", randomNumber.ToString());
            }
        }
        return original;
    }

    private static string ReplaceVariableWords(string original)
    {
        MatchCollection matches = Regex.Matches(original, @"\[w:(.*?)\]");
        Random random = new Random();

        foreach (Match match in matches)
        {
            string variableWordsPart = match.Groups[1].Value;
            string[] words = variableWordsPart.Split(',');

            int randomIndex = random.Next(0, words.Length);
            string randomWord = words[randomIndex];

            original = original.ReplaceFirst($"{match.Groups[0].Value}", randomWord);
        }
        return original;
    }
}