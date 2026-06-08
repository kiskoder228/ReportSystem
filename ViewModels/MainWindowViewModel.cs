using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _userName = "";

    [ObservableProperty]
    private string _userRole = "";

    [ObservableProperty]
    private object? _currentPage;

    public User CurrentUser { get; set; }

    public bool IsAdmin => CurrentUser.Role?.Name == "Admin";
    public bool IsStudent => CurrentUser.Role?.Name == "Student";
    public bool IsTeacher => CurrentUser.Role?.Name == "Teacher";

    private static readonly Dictionary<string, string> RoleLabels = new()
    {
        { "Admin", "Администратор" },
        { "Teacher", "Преподаватель" },
        { "Student", "Ученик" }
    };

    private readonly System.IServiceProvider _serviceProvider;

    public MainWindowViewModel(System.IServiceProvider serviceProvider, User user)
    {
        _serviceProvider = serviceProvider;
        CurrentUser = user;
        UserName = user.FullName;

        if (user.Role != null && RoleLabels.TryGetValue(user.Role.Name, out var label))
        {
            UserRole = label;
        }
        else if (user.Role != null)
        {
            UserRole = user.Role.Name;
        }
        else
        {
            UserRole = "Unknown";
        }

        Navigate("Dashboard");
    }

    [RelayCommand]
    public void Navigate(string destination)
    {
        if (destination == "Dashboard")
        {
            CurrentPage = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<DashboardViewModel>(_serviceProvider, CurrentUser);
        }
        else if (destination == "CreateReport" && IsStudent)
        {
            CurrentPage = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<CreateReportViewModel>(_serviceProvider, CurrentUser);
        }
        else if (destination == "MyReports" && IsStudent)
        {
            CurrentPage = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<MyReportsViewModel>(_serviceProvider, CurrentUser);
        }
        else if (destination == "AllReports" && (IsAdmin || IsTeacher))
        {
            CurrentPage = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<AdminReportsViewModel>(_serviceProvider, CurrentUser);
        }
        else if (destination == "Users" && IsAdmin)
        {
            CurrentPage = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<AdminUsersViewModel>(_serviceProvider, CurrentUser);
        }
    }
}