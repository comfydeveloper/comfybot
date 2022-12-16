namespace ComfyBot.Application.Output;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

[ExcludeFromCodeCoverage]
public class ConsoleOutputWriter : TextWriter
{
    private readonly List<string> lastOutput = new();

    public delegate void UpdateTextCallback(string message);

    private readonly TextBox textbox;

    public ConsoleOutputWriter(TextBox textbox)
    {
        this.textbox = textbox;
    }

    public override void Write(char value)
    {
        textbox.Dispatcher.Invoke(new UpdateTextCallback(UpdateText), new[] { value });
    }

    public override void Write(string value)
    {
        textbox.Dispatcher.Invoke(new UpdateTextCallback(UpdateText), new[] { value });
    }

    public override Encoding Encoding
    {
        get { return Encoding.ASCII; }
    }

    private void UpdateText(string message)
    {
        lastOutput.Add(message);

        if (lastOutput.Count > 100)
        {
            lastOutput.RemoveAt(0);
        }

        textbox.Text = string.Join(System.Environment.NewLine, lastOutput.Reverse<string>());
    }
}