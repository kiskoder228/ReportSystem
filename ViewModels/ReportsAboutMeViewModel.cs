using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class ReportsAboutMeViewModel : ObservableObject
{
    public ObservableCollection<Report> Reports { get; } = new();

    public string UserName { get; }
    public int UserScore { get; }
    public string UserRank { get; }
    public double ScorePercent { get; }

    [ObservableProperty]
    private bool _hasNoReports;

    public ReportsAboutMeViewModel(IReportRepository reportRepository, User user)
    {
        UserName = user.FullName;
        UserScore = user.Score;
        UserRank = user.Rank;
        // Полоска: уровень до следующего ранга
        // Подлиза 0-20, Смотрящий 21-50, Стукач 51-90, Крыса 91+
        ScorePercent = user.Score <= 0 ? 0
            : user.Score <= 20 ? (double)user.Score / 20 * 100
            : user.Score <= 50 ? (double)(user.Score - 20) / 30 * 100
            : user.Score <= 90 ? (double)(user.Score - 50) / 40 * 100
            : 100;

        try
        {
            var reports = reportRepository.GetReportsByViolator(user.Id);
            foreach (var r in reports)
                Reports.Add(r);
            HasNoReports = Reports.Count == 0;
        }
        catch
        {
            HasNoReports = true;
        }
    }
}
