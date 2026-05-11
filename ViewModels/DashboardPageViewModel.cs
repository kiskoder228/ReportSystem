using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class DashboardPageViewModel : ObservableObject
{
    private int _totalReports;
    private int _inProgressReports;
    private int _resolvedReports;
    private int _newReports;

    public int TotalReports { get => _totalReports; set => SetProperty(ref _totalReports, value); }
    public int NewReports { get => _newReports; set => SetProperty(ref _newReports, value); }
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
                var list = db.Reports.Include(r => r.Status).ToList();

                var dbUser = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == user.Id);

                if (dbUser?.Role?.Name == "Student")
                {
                    list = list.Where(r => r.AuthorId == user.Id).ToList();
                }

                TotalReports = list.Count();
                NewReports = list.Count(r => r.Status?.Title == "New");
                InProgressReports = list.Count(r => r.Status?.Title == "InProgress");
                ResolvedReports = list.Count(r => r.Status?.Title == "Resolved");
            }
        }
        catch (System.Exception)
        {
            // нули по умолчанию
        }
    }
}
