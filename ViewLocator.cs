using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReportSystem.ViewModels;
using ReportSystem.Views;

namespace ReportSystem;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        return data switch
        {
            DashboardPageViewModel vm    => new DashboardPage    { DataContext = vm },
            CreateReportViewModel vm     => new CreateReportPage { DataContext = vm },
            MyReportsViewModel vm        => new MyReportsPage    { DataContext = vm },
            AdminReportsViewModel vm     => new AdminReportsPage { DataContext = vm },
            AdminUsersViewModel vm       => new AdminUsersPage   { DataContext = vm },
            _ => new TextBlock { Text = "Страница не найдена: " + data?.GetType().Name }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}