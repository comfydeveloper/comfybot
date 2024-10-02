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
    private readonly List<string> lastOutput = [];

    public delegate void UpdateTextCallback(string message);

    private readonly TextBox textBox;

    public ConsoleOutputWriter(TextBox textBox)
    {
        this.textBox = textBox;
    }

    public override void Write(char value)
    {
        this.textBox.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), new[] { value });
    }

    public override void Write(string value)
    {
        this.textBox.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), value);
    }

    public override Encoding Encoding => Encoding.ASCII;

    private void UpdateText(string message)
    {
        this.lastOutput.Add(message);

        if (this.lastOutput.Count > 100)
        {
            this.lastOutput.RemoveAt(0);
        }

        this.textBox.Text = string.Join(Environment.NewLine, this.lastOutput.Reverse<string>());
    }
}