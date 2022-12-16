namespace ComfyBot.Application.Configuration;

using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

using Shared;
using Settings;

//TODO [comfy] Uncovered because application.tests project would simply not want to use the System.Configuration.ConfigurationManager.
[ExcludeFromCodeCoverage]
public class ConfigurationTabViewModel
{
    public string UserName { get; set; }

    public string Channel { get; set; }

    public string DatabasePath { get; set; }

    public string ChannelId { get; set; }

    public ConfigurationTabViewModel()
    {
        UpdateConfigurationCommand = new ParameterCommand(UpdateConfiguration);
        UserName = ApplicationSettings.Default.User;
        Channel = ApplicationSettings.Default.Channel;
        DatabasePath = ApplicationSettings.Default.DatabasePath;
        ChannelId = ApplicationSettings.Default.ChannelId;
    }

    public ParameterCommand UpdateConfigurationCommand { get; }

    public ParameterCommand UpdateWeatherApiKeyCommand { get; }

    private void UpdateConfiguration(object parameter)
    {
        ApplicationSettings.Default.ChannelId = ChannelId;
        ApplicationSettings.Default.AuthKey = ((PasswordBox)parameter).Password;
        ApplicationSettings.Default.User = UserName;
        ApplicationSettings.Default.Channel = Channel;
        ApplicationSettings.Default.DatabasePath = DatabasePath;
        ApplicationSettings.Default.Save();
    }
}