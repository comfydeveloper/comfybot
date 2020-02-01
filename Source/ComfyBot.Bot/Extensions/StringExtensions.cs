namespace ComfyBot.Bot.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        private const string RegularExpression = @"parameter(\d+)}}";

        public static bool CanHandleParameters(this string reply, int parameterCount)
        {
            MatchCollection matches = Regex.Matches(reply, RegularExpression);

            foreach (Match match in matches)
            {
                if (int.Parse(match.Groups[1].ToString()) > parameterCount)
                {
                    return false;
                }
            }
            return true;
        }
    }
}