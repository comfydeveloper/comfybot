using System.Windows;

namespace ComfyBot.Application
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Application.Output;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ConsoleOutputWriter writer = new ConsoleOutputWriter(this.ConsoleTextBox);
            Console.SetOut(writer);
        }
    }
}
