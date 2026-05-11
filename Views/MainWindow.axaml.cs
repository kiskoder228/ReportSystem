using Avalonia.Controls;
using ReportSystem.Models;
using ReportSystem.ViewModels;

namespace ReportSystem.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _vm;

    // Нужен для Avalonia XAML loader
    public MainWindow() : this(new Models.User { FullName = "Тест", Role = "Student" }) { }

    public MainWindow(User user)
    {
        InitializeComponent();
        _vm = new MainWindowViewModel(user);
        DataContext = _vm;

        // Ученик / Учитель
        WireStudentNav();
        // Администратор
        WireAdminNav();
        // Преподаватель
        WireTeacherNav();
        // Выход
        WireLogout();
    }

    private void WireTeacherNav()
    {
        var btnDash    = this.FindControl<Button>("NavTeacherDash");
        var btnReports = this.FindControl<Button>("NavTeacherReports");

        if (btnDash != null)
            btnDash.Click += (_, _) => { _vm.SelectedMenuIndex = 0; SetActive(btnDash, btnReports); };
        if (btnReports != null)
            btnReports.Click += (_, _) => { _vm.SelectedMenuIndex = 1; SetActive(btnReports, btnDash); };
    }

    private void WireStudentNav()
    {
        var btnDash   = this.FindControl<Button>("NavDashboard");
        var btnCreate = this.FindControl<Button>("NavCreate");
        var btnMy     = this.FindControl<Button>("NavMy");

        if (btnDash != null)
            btnDash.Click += (_, _) => { _vm.SelectedMenuIndex = 0; SetActive(btnDash, btnCreate, btnMy); };
        if (btnCreate != null)
            btnCreate.Click += (_, _) => { _vm.SelectedMenuIndex = 1; SetActive(btnCreate, btnDash, btnMy); };
        if (btnMy != null)
            btnMy.Click += (_, _) => { _vm.SelectedMenuIndex = 2; SetActive(btnMy, btnDash, btnCreate); };
    }

    private void WireAdminNav()
    {
        var btnDash    = this.FindControl<Button>("NavAdminDash");
        var btnReports = this.FindControl<Button>("NavAdminReports");
        var btnUsers   = this.FindControl<Button>("NavAdminUsers");

        if (btnDash != null)
            btnDash.Click += (_, _) => { _vm.SelectedMenuIndex = 0; SetActive(btnDash, btnReports, btnUsers); };
        if (btnReports != null)
            btnReports.Click += (_, _) => { _vm.SelectedMenuIndex = 1; SetActive(btnReports, btnDash, btnUsers); };
        if (btnUsers != null)
            btnUsers.Click += (_, _) => { _vm.SelectedMenuIndex = 2; SetActive(btnUsers, btnDash, btnReports); };
    }

    private void WireLogout()
    {
        var logoutBtn = this.FindControl<Button>("LogoutBtn");
        if (logoutBtn != null)
            logoutBtn.Click += (_, _) =>
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            };
    }

    private static void SetActive(Button active, params Button?[] others)
    {
        active.Classes.Remove("NavBtn");
        active.Classes.Add("NavBtnActive");
        foreach (var btn in others)
        {
            btn?.Classes.Remove("NavBtnActive");
            btn?.Classes.Add("NavBtn");
        }
    }
}