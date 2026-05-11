using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data;
using ReportSystem.Models;
using ReportSystem.Models.Enums;

namespace ReportSystem.ViewModels;

public class AdminReportsViewModel : ObservableObject
{
    private Report? _selectedReport;
    private string _searchText = "";
    private string? _filterStatus;

    public ObservableCollection<Report> Reports { get; } = new();

    public ObservableCollection<string> StatusFilters { get; } = new()
    {
        "Все", "New", "InProgress", "Resolved", "Rejected"
    };

    public Report? SelectedReport
    {
        get => _selectedReport;
        set => SetProperty(ref _selectedReport, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                LoadReports();
            }
        }
    }

    public string? FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
            {
                LoadReports();
            }
        }
    }

    public IRelayCommand<string> ChangeStatusCommand { get; }

    public AdminReportsViewModel(User user)
    {
        ChangeStatusCommand = new RelayCommand<string>(ChangeStatus);
        LoadReports();
    }

    private void LoadReports()
    {
        Reports.Clear();
        try
        {
            using (var db = new ApplicationDbContext())
            {
                // Загружаем всё из базы и фильтруем в памяти (так проще объяснить)
                var allReports = db.Reports.ToList();
                
                foreach (var r in allReports)
                {
                    // Простая фильтрация по тексту
                    if (!string.IsNullOrWhiteSpace(SearchText))
                    {
                        if (r.Description == null || !r.Description.ToLower().Contains(SearchText.ToLower()))
                            continue;
                    }

                    // Фильтрация по статусу
                    if (!string.IsNullOrWhiteSpace(FilterStatus) && FilterStatus != "Все")
                    {
                        if (r.Status.ToString() != FilterStatus)
                            continue;
                    }

                    // Подгружаем автора и категорию вручную
                    r.Author = db.Users.FirstOrDefault(u => u.Id == r.AuthorId);
                    r.Category = db.Categories.FirstOrDefault(c => c.Id == r.CategoryId);

                    Reports.Add(r);
                }
            }
        }
        catch (Exception)
        {
            // Ошибка загрузки
        }
    }

    private void ChangeStatus(string? statusStr)
    {
        if (SelectedReport == null || statusStr == null) return;
        if (!System.Enum.TryParse<ReportStatus>(statusStr, out var newStatus)) return;

        try
        {
            using (var db = new ApplicationDbContext())
            {
                var report = db.Reports.FirstOrDefault(r => r.Id == SelectedReport.Id);
                if (report != null)
                {
                    report.Status = newStatus;
                    if (newStatus == ReportStatus.3 || newStatus == ReportStatus.Rejected)
                        report.ResolvedAt = System.DateTime.UtcNow;

                    db.SaveChanges();
                    LoadReports();
                }
            }
        }
        catch (Exception)
        {
            
        }
    }
}
