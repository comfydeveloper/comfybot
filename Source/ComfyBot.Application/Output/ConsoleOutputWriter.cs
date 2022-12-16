namespace ComfyBot.Application.Output
{
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
            this.textbox.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), new[] { value });
        }

        public override void Write(string value)
        {
            this.textbox.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), new[] { value });
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }

        private void UpdateText(string message)
        {
            this.lastOutput.Add(message);

            if (this.lastOutput.Count > 100)
            {
                this.lastOutput.RemoveAt(0);
            }

            textbox.Text = string.Join(System.Environment.NewLine, this.lastOutput.Reverse<string>());
        }
    }
}