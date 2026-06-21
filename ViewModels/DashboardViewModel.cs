using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

// Вспомогательный класс для отображения рейтинга с полоской
public class InformantEntry
{
    public string FullName { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Address { get; set; } = string.Empty;
    public double ScorePercent { get; set; } // от 0 до 100
}

public partial class DashboardViewModel : ObservableObject
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;

    public ObservableCollection<InformantEntry> TopInformants { get; } = new();

    [ObservableProperty]
    private int _totalReports;

    [ObservableProperty]
    private int _newReports;

    [ObservableProperty]
    private int _inProgressReports;

    [ObservableProperty]
    private int _resolvedReports;

    public string GreetingName { get; }
    public bool IsStudent { get; }

    // Личный рейтинг нарушителя (только для студентов)
    public int MyScore { get; private set; }
    public string MyRank { get; private set; } = string.Empty;
    public double MyScorePercent { get; private set; }
    // Score уходит в минус — нормализуем: чем ниже, тем хуже
    // Диапазон: -100 (плохо) ... 0 ... +100 (нейтрально, очков нет)
    // Отображаем заполненность как «уровень репутации» от 0 до 100%

    public DashboardViewModel(IReportRepository reportRepository, IUserRepository userRepository, User user)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        GreetingName = user.FullName.Split(' ').FirstOrDefault() ?? user.FullName;
        IsStudent = user.Role?.Name == "Student";

        // Заполняем личный рейтинг нарушителя
        MyScore = user.Score;
        MyRank = user.Rank;
        // Полоска: чем больше отрицательный score, тем полнее заполнена
        // 0 очков = 0%, -100 и ниже = 100%
        MyScorePercent = user.Score >= 0 ? 0 : Math.Min(Math.Abs(user.Score), 100);

        int? authorId = user.Role?.Name == "Student" ? user.Id : null;

        try
        {
            TotalReports = _reportRepository.GetTotalCount(authorId);
            NewReports = _reportRepository.GetCountByStatus("Ожидает приговора", authorId);
            InProgressReports = _reportRepository.GetCountByStatus("В разработке", authorId);
            ResolvedReports = _reportRepository.GetCountByStatus("Виновен (Принято)", authorId);

            var informants = _userRepository.GetTopInformants(5).ToList();
            int maxScore = informants.Count > 0 ? Math.Max(informants.Max(u => u.Score), 1) : 1;

            foreach (var u in informants)
            {
                TopInformants.Add(new InformantEntry
                {
                    FullName = u.FullName,
                    Rank = u.Rank,
                    Score = u.Score,
                    Address = u.Address,
                    ScorePercent = (double)u.Score / maxScore * 100
                });
            }
        }
        catch { }
    }
}
