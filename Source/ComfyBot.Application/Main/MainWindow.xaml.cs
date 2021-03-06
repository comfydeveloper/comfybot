﻿using System.Windows;

namespace ComfyBot.Application
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using ComfyBot.Application.Main;
    using ComfyBot.Application.Output;
    using ComfyBot.Settings;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
            ConsoleOutputWriter writer = new ConsoleOutputWriter(this.ConsoleTextBox);
            Console.SetOut(writer);

            this.StreamKey.Password = ApplicationSettings.Default.AuthKey;
            this.WeatherApiKey.Password = ApplicationSettings.Default.OpenWeatherMapApiKey;
        }
    }
}
