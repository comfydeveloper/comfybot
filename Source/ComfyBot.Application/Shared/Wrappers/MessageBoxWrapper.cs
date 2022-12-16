using System.Windows;

namespace ComfyBot.Application.Shared.Wrappers;

public class MessageBoxWrapper : IMessageBox
{
    public MessageBoxResult Show(string message, string caption, MessageBoxButton button)
    {
        return MessageBox.Show(message, caption, button);
    }
}