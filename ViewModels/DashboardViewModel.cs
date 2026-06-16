using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;

    public ObservableCollection<User> TopInformants { get; } = new();

    [ObservableProperty]
    private int _totalReports;

    [ObservableProperty]
    private int _newReports;

    [ObservableProperty]
    private int _inProgressReports;

    [ObservableProperty]
    private int _resolvedReports;

    public string GreetingName { get; }

    public DashboardViewModel(IReportRepository reportRepository, IUserRepository userRepository, User user)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        GreetingName = user.FullName.Split(' ').FirstOrDefault() ?? user.FullName;

        int? authorId = user.Role?.Name == "Student" ? user.Id : null;

        try
        {
            TotalReports = _reportRepository.GetTotalCount(authorId);
            NewReports = _reportRepository.GetCountByStatus("Ожидает приговора", authorId);
            InProgressReports = _reportRepository.GetCountByStatus("В разработке", authorId);
            ResolvedReports = _reportRepository.GetCountByStatus("Виновен (Принято)", authorId);

            foreach(var informant in _userRepository.GetTopInformants(5))
            {
                TopInformants.Add(informant);
            }
        }
        catch { }
    }
}
