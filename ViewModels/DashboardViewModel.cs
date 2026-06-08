using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IReportRepository _reportRepository;

    [ObservableProperty]
    private int _totalReports;

    [ObservableProperty]
    private int _newReports;

    [ObservableProperty]
    private int _inProgressReports;

    [ObservableProperty]
    private int _resolvedReports;

    public string GreetingName { get; }

    public DashboardViewModel(IReportRepository reportRepository, User user)
    {
        _reportRepository = reportRepository;
        GreetingName = user.FullName.Split(' ').FirstOrDefault() ?? user.FullName;

        int? authorId = user.Role?.Name == "Student" ? user.Id : null;

        try
        {
            TotalReports = _reportRepository.GetTotalCount(authorId);
            NewReports = _reportRepository.GetCountByStatus("New", authorId);
            InProgressReports = _reportRepository.GetCountByStatus("InProgress", authorId);
            ResolvedReports = _reportRepository.GetCountByStatus("Resolved", authorId);
        }
        catch { }
    }
}
