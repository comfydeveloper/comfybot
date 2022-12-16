using System.Windows;

namespace ComfyBot.Application.Shared.Wrappers;

public interface IMessageBox
{
    MessageBoxResult Show(string message, string caption, MessageBoxButton button);
}