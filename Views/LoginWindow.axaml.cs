using Avalonia.Controls;
using ReportSystem.Models;
using ReportSystem.ViewModels;

namespace ReportSystem.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        var vm = new LoginViewModel();
        DataContext = vm;

        // При успешном входе — открываем главное окно
        vm.LoginSucceeded += OnLoginSucceeded;

        // Кнопка «Зарегистрироваться»
        var registerBtn = this.FindControl<Button>("OpenRegisterBtn");
        if (registerBtn != null)
            registerBtn.Click += (_, _) =>
            {
                var regWindow = new RegisterWindow();
                regWindow.Show();
                this.Hide();
                regWindow.Closed += (_, _) => this.Show();
            };
    }

    private void OnLoginSucceeded(User user)
    {
        var mainWindow = new MainWindow(user);
        mainWindow.Show();
        this.Close();
    }
}