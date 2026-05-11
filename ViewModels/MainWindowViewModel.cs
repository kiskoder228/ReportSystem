using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private string _userName = "";
    private string _userRole = "";
    private object? _currentPage;
    private int _selectedMenuIndex = 0;

    public User CurrentUser { get; }

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string UserRole
    {
        get => _userRole;
        set => SetProperty(ref _userRole, value);
    }

    // Текущая страница (контент)
    public object? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public int SelectedMenuIndex
    {
        get => _selectedMenuIndex;
        set
        {
            SetProperty(ref _selectedMenuIndex, value);
            NavigateTo(value);
        }
    }

    public bool IsAdmin => CurrentUser.Role?.Name == "Admin";
    public bool IsStudent => CurrentUser.Role?.Name == "Student";
    public bool IsTeacher => CurrentUser.Role?.Name == "Teacher";

    public MainWindowViewModel(User user)
    {
        CurrentUser = user;
        UserName = user.FullName;
        UserRole = user.Role?.Name switch
        {
            "Admin" => "Администратор",
            "Teacher" => "Преподаватель",
            _ => "Ученик"
        };

        // Открываем главную страницу
        NavigateTo(1);
    }

    private void NavigateTo(int index)
    {
        if (IsAdmin)
        {
            CurrentPage = index switch
            {
                0 => new DashboardPageViewModel(CurrentUser),
                1 => new AdminReportsViewModel(CurrentUser),
                2 => new AdminUsersViewModel(CurrentUser),
                _ => null
            };
        }
        else if (IsTeacher)
        {
            CurrentPage = index switch
            {
                0 => new DashboardPageViewModel(CurrentUser),
                1 => new AdminReportsViewModel(CurrentUser), // Преподаватель видит то же, что админ, но без управления ролями
                _ => null
            };
        }
        else
        {
            CurrentPage = index switch
            {
                0 => new DashboardPageViewModel(CurrentUser),
                1 => new CreateReportViewModel(CurrentUser),
                2 => new MyReportsViewModel(CurrentUser),
                _ => null
            };
        }
    }
}