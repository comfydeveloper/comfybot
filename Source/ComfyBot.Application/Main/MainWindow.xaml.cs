using System.Windows;

namespace ComfyBot.Application
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Main;
    using Output;
    using Settings;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
            ConsoleOutputWriter writer = new ConsoleOutputWriter(ConsoleTextBox);
            Console.SetOut(writer);

            StreamKey.Password = ApplicationSettings.Default.AuthKey;
        }
    }
}
