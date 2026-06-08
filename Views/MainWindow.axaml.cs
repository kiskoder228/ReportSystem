using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReportSystem.ViewModels;
using ReportSystem.Models;

namespace ReportSystem.Views;

public partial class MainWindow : Window
{
    private readonly IServiceProvider? _serviceProvider;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(IServiceProvider serviceProvider, User user)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        DataContext = ActivatorUtilities.CreateInstance<MainWindowViewModel>(serviceProvider, user);
    }

    private void OnLogoutClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_serviceProvider != null)
        {
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
            this.Close();
        }
    }
}