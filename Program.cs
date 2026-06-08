using Avalonia;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ReportSystem.Views;
using ReportSystem.ViewModels;
using ReportSystem.Data;
using ReportSystem.Data.Repositories;

namespace ReportSystem;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args).
            ConfigureAppConfiguration((context, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
            }).
            ConfigureServices((context, services) =>
            {
                services.AddDbContextFactory<ApplicationDbContext>();

                // Repositories
                services.AddTransient<ICategoryRepository, CategoryRepository>();
                services.AddTransient<IUserRepository, UserRepository>();
                services.AddTransient<IReportRepository, ReportRepository>();
                
                // Views (Windows and UserControls)
                services.AddTransient<LoginWindow>();
                services.AddTransient<RegisterWindow>();
                services.AddTransient<MainWindow>();
                services.AddTransient<DashboardView>();
                services.AddTransient<CreateReportView>();
                services.AddTransient<MyReportsView>();
                services.AddTransient<AdminReportsView>();
                services.AddTransient<AdminUsersView>();
                
                // ViewModels
                services.AddTransient<LoginViewModel>();
                services.AddTransient<RegisterViewModel>();
                services.AddTransient<MainWindowViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<CreateReportViewModel>();
                services.AddTransient<MyReportsViewModel>();
                services.AddTransient<AdminReportsViewModel>();
                services.AddTransient<AdminUsersViewModel>();
            }).
            Build();

        BuildAvaloniaApp(host.Services)
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider)
        => AppBuilder.Configure(() => new App(serviceProvider))
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}