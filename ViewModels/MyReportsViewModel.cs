using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Data;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class MyReportsViewModel : ViewModelBase
{
    public ObservableCollection<Report> Reports { get; } = new();

    public MyReportsViewModel(User user)
    {
        LoadReports(user);
    }

    private void LoadReports(User user)
    {
        try
        {
            using var db = new ApplicationDbContext();

            var list = db.Reports
                .Include(r => r.Category)
                .Include(r => r.Status)
                .Where(r => r.AuthorId == user.Id)
                .ToList();

            foreach (var report in list)
                Reports.Add(report);
        }
        catch (System.Exception) { }
    }
}
