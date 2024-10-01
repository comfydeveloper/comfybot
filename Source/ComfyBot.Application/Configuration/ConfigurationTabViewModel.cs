using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using ComfyBot.Application.Shared;
using ComfyBot.Settings;

namespace ComfyBot.Application.Configuration;

// TODO [Shae] Remove this + configuration project
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
        this.UpdateConfigurationCommand = new ParameterCommand(this.UpdateConfiguration);
        this.UserName = ApplicationSettings.Default.User;
        this.Channel = ApplicationSettings.Default.Channel;
        this.DatabasePath = ApplicationSettings.Default.DatabasePath;
        this.ChannelId = ApplicationSettings.Default.ChannelId;
    }

    public ParameterCommand UpdateConfigurationCommand { get; }

    private void UpdateConfiguration(object parameter)
    {
        ApplicationSettings.Default.ChannelId = this.ChannelId;
        ApplicationSettings.Default.AuthKey = ((PasswordBox)parameter).Password;
        ApplicationSettings.Default.User = this.UserName;
        ApplicationSettings.Default.Channel = this.Channel;
        ApplicationSettings.Default.DatabasePath = this.DatabasePath;
        ApplicationSettings.Default.Save();
    }
}