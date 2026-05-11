using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data;
using ReportSystem.Models;
using BC = BCrypt.Net.BCrypt;

namespace ReportSystem.ViewModels;

public class RegisterViewModel : ObservableObject
{
    private string _fullName = "";
    private string _login = "";
    private string _password = "";
    private string _statusMessage = "";
    private IBrush _messageColor = Brushes.Red;
    private bool _showPassword;

    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    public string Login { get => _login; set => SetProperty(ref _login, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public bool ShowPassword { get => _showPassword; set => SetProperty(ref _showPassword, value); }
    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            SetProperty(ref _statusMessage, value);
            OnPropertyChanged(nameof(HasMessage));
        }
    }

    public bool HasMessage => !string.IsNullOrEmpty(_statusMessage);
    public IBrush MessageColor { get => _messageColor; set => SetProperty(ref _messageColor, value); }
    // Роль при регистрации всегда "Student"
    public string SelectedRole { get; } = "Student";

    public IRelayCommand RegisterCommand { get; }

    public RegisterViewModel()
    {
        RegisterCommand = new RelayCommand(OnRegister);
    }

    private void OnRegister()
    {
        if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(FullName))
        {
            StatusMessage = "Заполните все поля!";
            MessageColor = Brushes.Red;
            return;
        }

        try
        {
            using (var db = new ApplicationDbContext())
            {
                // Проверяем есть ли такой логин
                if (db.Users.Any(u => u.Login == Login))
                {
                    StatusMessage = "Логин уже занят!";
                    MessageColor = Brushes.Red;
                    return;
                }

                var role = db.Roles.FirstOrDefault(r => r.Name == SelectedRole);

                // Создаем нового юзера
                var newUser = new User
                {
                    FullName = FullName,
                    Login = Login,
                    PasswordHash = BC.HashPassword(Password), // хэшируем пароль для безопасности
                    Role = role
                };

                db.Users.Add(newUser);
                db.SaveChanges(); // сохраняем

                StatusMessage = "Регистрация успешна!";
                MessageColor = Brushes.Green;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Ошибка: " + ex.Message;
            MessageColor = Brushes.Red;
        }
    }
}
