using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ComfyBot.Application.Main;
using ComfyBot.Application.Output;

namespace ComfyBot.Application;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExcludeFromCodeCoverage]
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        this.DataContext = viewModel;

        this.InitializeComponent();
        ConsoleOutputWriter writer = new(this.ConsoleTextBox);
        Console.SetOut(writer);
    }
}