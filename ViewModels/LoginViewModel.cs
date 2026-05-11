using System;
using System.Linq;
using System.Threading.Tasks;
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

    // Нужно для IsVisible в AXAML (!! не работает в Avalonia с compiled bindings)
    public bool HasMessage => !string.IsNullOrEmpty(_errorMessage);

    public IRelayCommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    // Событие успешного входа — передаём пользователя наружу (в codebehind)
    public event Action<Models.User>? LoginSucceeded;

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
            // Подключаемся к базе
            using (var db = new ApplicationDbContext())
            {
                // Ищем юзера по логину
                var user = db.Users.FirstOrDefault(u => u.Login == Login);

                // Если юзер есть и пароль совпал (проверяем хэш)
                if (user != null && BC.Verify(Password, user.PasswordHash))
                {
                    // Успешный вход!
                    LoginSucceeded?.Invoke(user);
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Ошибка: " + ex.Message;
        }
    }
}