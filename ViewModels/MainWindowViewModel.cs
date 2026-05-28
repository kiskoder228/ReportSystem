using System.Collections.Generic;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class MainWindowViewModel : ViewModelBase
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
        { "Admin", "Администратор" },
        { "Teacher", "Преподаватель" },
        { "Student", "Ученик" }
    };

    public MainWindowViewModel(User user)
    {
        CurrentUser = user;
        UserName = user.FullName;

        if (user.Role != null && RoleLabels.ContainsKey(user.Role.Name))
        {
            UserRole = RoleLabels[user.Role.Name];
        }
        else if (user.Role != null)
        {
            UserRole = user.Role.Name;
        }
        else
        {
            UserRole = "Unknown";
        }

        NavigateTo(1);
    }

    private void NavigateTo(int index)
    {
        if (IsAdmin)
        {
            if (index == 0) CurrentPage = new DashboardPageViewModel(CurrentUser);
            else if (index == 1) CurrentPage = new AdminReportsViewModel(CurrentUser);
            else if (index == 2) CurrentPage = new AdminUsersViewModel(CurrentUser);
        }
        else if (IsTeacher)
        {
            if (index == 0) CurrentPage = new DashboardPageViewModel(CurrentUser);
            else if (index == 1) CurrentPage = new AdminReportsViewModel(CurrentUser);
        }
        else if (IsStudent)
        {
            if (index == 0) CurrentPage = new DashboardPageViewModel(CurrentUser);
            else if (index == 1) CurrentPage = new CreateReportViewModel(CurrentUser);
            else if (index == 2) CurrentPage = new MyReportsViewModel(CurrentUser);
        }
    }
}