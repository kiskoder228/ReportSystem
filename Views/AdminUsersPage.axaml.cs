using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ReportSystem.Views;

public partial class AdminUsersPage : UserControl
{
    public AdminUsersPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
