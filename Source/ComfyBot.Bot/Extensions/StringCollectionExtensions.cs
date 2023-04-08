using System;
using System.Collections.Generic;
using System.Linq;

namespace ComfyBot.Bot.Extensions;

public static class StringCollectionExtensions
{
    public static string GetRandom(this IEnumerable<string> collection)
    {
        string[] strings = collection.ToArray();

        if (strings.Length == 1)
        {
            return strings[0];
        }

        Random random = new Random();
        int index = random.Next(0, strings.Length);

        return strings[index];
    }
}