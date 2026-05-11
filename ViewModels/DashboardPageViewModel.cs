using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Data;
using ReportSystem.Models;
using ReportSystem.Models.Enums;

namespace ReportSystem.ViewModels;

public class DashboardPageViewModel : ObservableObject
{
    private int _totalReports;
    private int _inProgressReports;
    private int _resolvedReports;
    private int _newReports;

    public int TotalReports   { get => _totalReports;    set => SetProperty(ref _totalReports, value); }
    public int NewReports     { get => _newReports;      set => SetProperty(ref _newReports, value); }
    public int InProgressReports { get => _inProgressReports; set => SetProperty(ref _inProgressReports, value); }
    public int ResolvedReports { get => _resolvedReports; set => SetProperty(ref _resolvedReports, value); }

    public string GreetingName { get; }

    public DashboardPageViewModel(User user)
    {
        GreetingName = user.FullName.Split(' ')[0]; // Только имя

        try
        {
            using (var db = new ApplicationDbContext())
            {
                // Загружаем список для подсчета (для студента только его, для остальных всё)
                var list = db.Reports.ToList();
                if (user.Role == "Student")
                {
                    list = list.Where(r => r.AuthorId == user.Id).ToList();
                }

                TotalReports      = list.Count();
                NewReports        = list.Count(r => r.Status == ReportStatus.1);
                InProgressReports = list.Count(r => r.Status == ReportStatus.2);
                ResolvedReports   = list.Count(r => r.Status == ReportStatus.3);
            }
        }
        catch (System.Exception)
        {
            // нули по умолчанию
        }
    }
}
