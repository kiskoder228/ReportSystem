using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data.Repositories;

namespace ReportSystem.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;

    [ObservableProperty]
    private string _login = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _rememberMe;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMessage))]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _showPassword;

    public bool HasMessage => !string.IsNullOrEmpty(ErrorMessage);

    public event Action<Models.User>? LoginSucceeded;

    public LoginViewModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [RelayCommand]
    private void DoLogin()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля!";
            return;
        }

        try
        {
            var user = _userRepository.ValidateUser(Login, Password);
            if (user != null)
            {
                LoginSucceeded?.Invoke(user);
            }
            else
            {
                ErrorMessage = "Неверный логин или пароль";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Ошибка: " + ex.Message;
        }
    }
}