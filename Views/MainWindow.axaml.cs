using Avalonia.Controls;
using ReportSystem.Models;
using ReportSystem.ViewModels;

namespace ReportSystem.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _vm;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(User user)
    {
        InitializeComponent();
        _vm = new MainWindowViewModel(user);
        DataContext = _vm;

        var btnDash = this.FindControl<Button>("NavDashboard");
        var btnCreate = this.FindControl<Button>("NavCreate");
        var btnMy = this.FindControl<Button>("NavMy");
        
        if (btnDash != null) btnDash.Click += (s, e) => _vm.SelectedMenuIndex = 0;
        if (btnCreate != null) btnCreate.Click += (s, e) => _vm.SelectedMenuIndex = 1;
        if (btnMy != null) btnMy.Click += (s, e) => _vm.SelectedMenuIndex = 2;

        var btnTeacherDash = this.FindControl<Button>("NavTeacherDash");
        var btnTeacherReports = this.FindControl<Button>("NavTeacherReports");
        
        if (btnTeacherDash != null) btnTeacherDash.Click += (s, e) => _vm.SelectedMenuIndex = 0;
        if (btnTeacherReports != null) btnTeacherReports.Click += (s, e) => _vm.SelectedMenuIndex = 1;

        var btnAdminDash = this.FindControl<Button>("NavAdminDash");
        var btnAdminReports = this.FindControl<Button>("NavAdminReports");
        var btnAdminUsers = this.FindControl<Button>("NavAdminUsers");

        if (btnAdminDash != null) btnAdminDash.Click += (s, e) => _vm.SelectedMenuIndex = 0;
        if (btnAdminReports != null) btnAdminReports.Click += (s, e) => _vm.SelectedMenuIndex = 1;
        if (btnAdminUsers != null) btnAdminUsers.Click += (s, e) => _vm.SelectedMenuIndex = 2;

        var logoutBtn = this.FindControl<Button>("LogoutBtn");
        if (logoutBtn != null)
        {
            logoutBtn.Click += (s, e) =>
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            };
        }
    }
}