using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ComfyBot.Application.Main;
using ComfyBot.Application.Output;
using ComfyBot.Settings;

namespace ComfyBot.Application;

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