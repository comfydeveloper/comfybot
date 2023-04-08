using System;

namespace ComfyBot.Settings.Extensions;

public static class EnvironmentExtensions
{
    public static string GetDatabasePath()
    {
        return Environment.ExpandEnvironmentVariables(ApplicationSettings.Default.DatabasePath);
    }
}