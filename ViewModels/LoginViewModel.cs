using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data;
using BC = BCrypt.Net.BCrypt;

namespace ReportSystem.ViewModels;

public class LoginViewModel : ObservableObject
{
    private string _login = string.Empty;
    private string _password = string.Empty;
    private bool _rememberMe;
    private string? _errorMessage;
    private bool _showPassword;

    public string Login
    {
        get => _login;
        set => SetProperty(ref _login, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public bool ShowPassword
    {
        get => _showPassword;
        set => SetProperty(ref _showPassword, value);
    }

    public bool RememberMe
    {
        get => _rememberMe;
        set => SetProperty(ref _rememberMe, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            OnPropertyChanged(nameof(HasMessage));
        }
    }

    public bool HasMessage => !string.IsNullOrEmpty(_errorMessage);

    public IRelayCommand LoginCommand { get; }

    public event Action<Models.User>? LoginSucceeded;

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    private void ExecuteLogin()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля!";
            return;
        }

        try
        {
            using var db = new ApplicationDbContext();

            var user = db.Users.FirstOrDefault(u => u.Login == Login);

            if (user != null && BC.Verify(Password, user.PasswordHash))
                LoginSucceeded?.Invoke(user);
            else
                ErrorMessage = "Неверный логин или пароль";
        }
        catch (Exception ex)
        {
            ErrorMessage = "Ошибка: " + ex.Message;
        }
    }
}