using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class MyReportsViewModel : ObservableObject
{
    private readonly IReportRepository _reportRepository;

    public ObservableCollection<Report> Reports { get; } = new();

    public MyReportsViewModel(IReportRepository reportRepository, User user)
    {
        _reportRepository = reportRepository;
        LoadReports(user);
    }

    private void LoadReports(User user)
    {
        try
        {
            var list = _reportRepository.GetReportsByAuthor(user.Id);
            foreach (var report in list)
                Reports.Add(report);
        }
        catch { }
    }
}
