using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ComfyBot.Application.Shared.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<TextModel> ToTextModels(this IEnumerable<string> collection)
    {
        return collection.Select(s => new TextModel { Text = s });
    }

    public static string[] ToStrings(this IEnumerable<TextModel> collection)
    {
        return collection.Where(x => !string.IsNullOrEmpty(x.Text)).Select(x => x.Text).ToArray();
    }
}