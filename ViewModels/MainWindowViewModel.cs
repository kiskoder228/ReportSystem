using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private string _userName = "";
    private string _userRole = "";
    private object? _currentPage;
    private int _selectedMenuIndex = 0;

    public User CurrentUser { get; set; }

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

    private static readonly Dictionary<string, string> RoleLabels = new()
    {
        ["Admin"] = "Администратор",
        ["Teacher"] = "Преподаватель",
        ["Student"] = "Ученик"
    };

    public MainWindowViewModel(User user)
    {
        CurrentUser = user;
        UserName = user.FullName;
        UserRole = user.Role != null && RoleLabels.TryGetValue(user.Role.Name, out var label) ? label : (user.Role?.Name ?? "Unknown");
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
                2 => (object)new AdminUsersViewModel(CurrentUser),
                _ => CurrentPage
            };
        }
        else if (IsTeacher)
        {
            CurrentPage = index switch
            {
                0 => new DashboardPageViewModel(CurrentUser),
                1 => (object)new AdminReportsViewModel(CurrentUser),
                _ => CurrentPage
            };
        }
        else if (IsStudent)
        {
            CurrentPage = index switch
            {
                0 => new DashboardPageViewModel(CurrentUser),
                1 => new CreateReportViewModel(CurrentUser),
                2 => (object)new MyReportsViewModel(CurrentUser),
                _ => CurrentPage
            };
        }
    }
}