using Avalonia.Controls;
using ReportSystem.ViewModels;

namespace ReportSystem.Views;

public partial class RegisterWindow : Window
{
    public RegisterWindow()
    {
        InitializeComponent();
        DataContext = new RegisterViewModel();

        // Кнопка «Войти» — закрыть RegisterWindow
        var backBtn = this.FindControl<Button>("BackToLoginBtn");
        if (backBtn != null)
            backBtn.Click += (_, _) => this.Close();
    }
}
