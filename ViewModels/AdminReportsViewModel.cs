using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class AdminReportsViewModel : ObservableObject
{
    private readonly IReportRepository _reportRepository;

    [ObservableProperty]
    private Report? _selectedReport;

    [ObservableProperty]
    private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        LoadReports();
    }

    [ObservableProperty]
    private string? _filterStatus;

    partial void OnFilterStatusChanged(string? value)
    {
        LoadReports();
    }

    public ObservableCollection<Report> Reports { get; } = new();

    public ObservableCollection<string> StatusFilters { get; } = new()
    {
        "Все", "New", "InProgress", "Resolved", "Rejected"
    };

    public AdminReportsViewModel(IReportRepository reportRepository, User user)
    {
        _reportRepository = reportRepository;
        LoadReports();
    }

    private void LoadReports()
    {
        Reports.Clear();
        try
        {
            var query = _reportRepository.GetAllReports(SearchText, FilterStatus);
            foreach (var report in query)
            {
                Reports.Add(report);
            }
        }
        catch { }
    }

    [RelayCommand]
    private void ChangeStatus(string? statusStr)
    {
        if (SelectedReport == null || statusStr == null) return;

        try
        {
            _reportRepository.UpdateStatus(SelectedReport.Id, statusStr);
            LoadReports();
        }
        catch { }
    }
}
