using ComfyBot.Bot.Extensions;
using ComfyBot.Data.Models;
using ComfyBot.Data.Repositories;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComfyBot.Bot.ChatBot.Services.Strategies;

public class VariableStrategy : IReplacementStrategy
{
    private readonly IQueryableRepository repository;

    public VariableStrategy(IQueryableRepository repository)
    {
        this.repository = repository;
    }

    public string Replace(string text, WildcardReplacerOptions options)
    {
        text = this.ReplacePlainVariables(text);
        text = this.ReplaceIncrementalVariables(text);

        return text;
    }

    private string ReplaceIncrementalVariables(string text)
    {
        MatchCollection matches = Regex.Matches(text, @"\[v\+\+:(.*?)\]");

        foreach (Match match in matches)
        {
            string variableName = match.Groups[1].Value;
            Variable variable = this.repository.Query<Variable>().FirstOrDefault(x => x.Name.ToLower() == variableName.ToLower());

            if (variable == null)
            {
                continue;
            }

            if(int.TryParse(variable.Value, out int numericValue))
            {
                numericValue++;
                variable.Value = numericValue.ToString();
                text = text.ReplaceFirst($"{match.Groups[0].Value}", variable.Value);
            }
        }

        return text;
    }

    private string ReplacePlainVariables(string text)
    {
        MatchCollection matches = Regex.Matches(text, @"\[v:(.*?)\]");

        foreach (Match match in matches)
        {
            string variableName = match.Groups[1].Value;
            Variable variable = this.repository.Query<Variable>().FirstOrDefault(x => x.Name.ToLower() == variableName.ToLower());
            if (variable != null)
            {
                text = text.ReplaceFirst($"{match.Groups[0].Value}", variable.Value);
            }
        }

        return text;
    }
}