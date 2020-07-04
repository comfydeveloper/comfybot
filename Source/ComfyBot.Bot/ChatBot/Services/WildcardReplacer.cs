namespace ComfyBot.Bot.ChatBot.Services
{
    using System;
    using System.Text.RegularExpressions;

    using ComfyBot.Bot.ChatBot.Chatters;
    using ComfyBot.Bot.Extensions;

    public class WildcardReplacer : IWildcardReplacer
    {
        private readonly IChattersCache chattersCache;

        public WildcardReplacer(IChattersCache chattersCache)
        {
            this.chattersCache = chattersCache;
        }

        public string Replace(string original)
        {
            string result = ReplaceVariableWords(original);
            result = ReplaceNumberRange(result);
            result = ReplaceRandomChatter(result);

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

        private string ReplaceRandomChatter(string original)
        {
            MatchCollection matches = Regex.Matches(original, "{{chatter}}");

            foreach (Match match in matches)
            {
                string randomChatter = this.chattersCache.GetRandom();

                original = original.ReplaceFirst($"{match.Groups[0].Value}", randomChatter);
            }

            return original;
        }
    }
}