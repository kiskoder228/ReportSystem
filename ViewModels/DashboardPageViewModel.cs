using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class DashboardPageViewModel : ViewModelBase
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
        GreetingName = user.FullName.Split(' ').FirstOrDefault() ?? user.FullName;

        try
        {
            using var db = new ApplicationDbContext();

            var dbUser = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == user.Id);

            var reports = db.Reports.Include(r => r.Status).AsEnumerable();

            if (dbUser?.Role?.Name == "Student")
                reports = reports.Where(r => r.AuthorId == user.Id);

            var list = reports.ToList();

            TotalReports = list.Count;
            NewReports = list.Count(r => r.Status?.Name == "New");
            InProgressReports = list.Count(r => r.Status?.Name == "InProgress");
            ResolvedReports = list.Count(r => r.Status?.Name == "Resolved");
        }
        catch (System.Exception) { }
    }
}
