using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ComfyBot.Application.Output;

[ExcludeFromCodeCoverage]
public class ConsoleOutputWriter : TextWriter
{
    private readonly List<string> lastOutput = new();

    public delegate void UpdateTextCallback(string message);

    private readonly TextBox textBox;

    public ConsoleOutputWriter(TextBox textBox)
    {
        this.textBox = textBox;
    }

    public override void Write(char value)
    {
        textBox.Dispatcher.Invoke(new UpdateTextCallback(UpdateText), new[] { value });
    }

    public override void Write(string value)
    {
        textBox.Dispatcher.Invoke(new UpdateTextCallback(UpdateText), value);
    }

    public override Encoding Encoding => Encoding.ASCII;

    private void UpdateText(string message)
    {
        lastOutput.Add(message);

        if (lastOutput.Count > 100)
        {
            lastOutput.RemoveAt(0);
        }

        textBox.Text = string.Join(Environment.NewLine, lastOutput.Reverse<string>());
    }
}