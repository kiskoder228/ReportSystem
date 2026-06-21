using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data.Repositories;

namespace ReportSystem.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _login = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _address = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMessage))]
    private string _statusMessage = "";

    [ObservableProperty]
    private IBrush _messageColor = Brushes.Red;

    [ObservableProperty]
    private bool _showPassword;

    public bool HasMessage => !string.IsNullOrEmpty(StatusMessage);

    public string SelectedRole { get; } = "Student";

    public event Action<Models.User>? RegisterSucceeded;

    public RegisterViewModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    private void ShowMessage(string message, IBrush color)
    {
        StatusMessage = message;
        MessageColor = color;
    }

    [RelayCommand]
    private void Register()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(FullName))
        {
            ShowMessage("Заполните все поля!", Brushes.Red);
            return;
        }

        if (string.IsNullOrWhiteSpace(Address))
        {
            ShowMessage("Укажите место жительства!", Brushes.Red);
            return;
        }

        try
        {
            if (_userRepository.UserExists(Login))
            {
                ShowMessage("Логин уже занят!", Brushes.Red);
                return;
            }

            var newUser = _userRepository.RegisterUser(FullName, Login, Password, Address);

            ShowMessage("Регистрация успешна!", Brushes.Green);
            
            RegisterSucceeded?.Invoke(newUser);
        }
        catch (Exception ex)
        {
            ShowMessage("Ошибка: " + ex.Message, Brushes.Red);
        }
    }
}
