using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ReportSystem.Data;
using ReportSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ReportSystem.ViewModels;

public class AdminReportsViewModel : ViewModelBase
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
                LoadReports();
        }
    }

    public string? FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
                LoadReports();
        }
    }

    public ICommand ChangeStatusCommand { get; }

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
            using var db = new ApplicationDbContext();

            var query = db.Reports
                .Include(r => r.Status)
                .Include(r => r.Author)
                .Include(r => r.Category)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(r => r.Description != null &&
                                         r.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(FilterStatus) && FilterStatus != "Все")
                query = query.Where(r => r.Status?.Name == FilterStatus);

            foreach (var report in query)
            {
                Reports.Add(report);
            }
        }
        catch (Exception) { }
    }

    private void ChangeStatus(string? statusStr)
    {
        if (SelectedReport == null || statusStr == null) return;

        try
        {
            using var db = new ApplicationDbContext();

            var newStatus = db.ReportStatuses.FirstOrDefault(s => s.Name == statusStr);
            if (newStatus == null) return;

            var report = db.Reports.FirstOrDefault(r => r.Id == SelectedReport.Id);
            if (report == null) return;

            report.StatusId = newStatus.Id;

            if (newStatus.Name == "Resolved" || newStatus.Name == "Rejected")
                report.ResolvedAt = DateTime.UtcNow;

            db.SaveChanges();
            LoadReports();
        }
        catch (Exception) { }
    }
}
