using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReportSystem.Models;
using ReportSystem.ViewModels;

namespace ReportSystem.Views;

public partial class LoginWindow : Window
{
    private readonly IServiceProvider? _serviceProvider;

    public LoginWindow()
    {
        InitializeComponent();
    }

    public LoginWindow(IServiceProvider serviceProvider, LoginViewModel vm)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        DataContext = vm;

        vm.LoginSucceeded += OnLoginSucceeded;
    }

    private void OnRegisterClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_serviceProvider != null)
        {
            var regWindow = _serviceProvider.GetRequiredService<RegisterWindow>();
            regWindow.Show();
            this.Hide();
            regWindow.Closed += (_, _) => this.Show();
        }
    }

    private void OnLoginSucceeded(User user)
    {
        if (_serviceProvider != null)
        {
            var mainWindow = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<MainWindow>(_serviceProvider, user);
            mainWindow.Show();
            this.Close();
        }
    }
}