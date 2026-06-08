using Avalonia.Controls;
using ReportSystem.ViewModels;

namespace ReportSystem.Views;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
    }

    public RegisterWindow(RegisterViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void OnBackClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}
