using System;
using System.Linq;
using System.Reflection;
using ComfyBot.Bot.ChatBot.Commands;
using ComfyBot.Common.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ComfyBot.Application;

// TODO [Shae] Remove this 
[Obsolete]
public class Startup
{
    public static void Initialize()
    {
        AssertDatabaseDirectoryExists();
    }

    private static void AssertDatabaseDirectoryExists()
    {
        // TODO [Shae] Remove/Assure this in another place
        //var databasePath = EnvironmentExtensions.GetDatabasePath();
        //Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
    }
}