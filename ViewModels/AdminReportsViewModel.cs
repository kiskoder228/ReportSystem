using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class AdminReportsViewModel : ObservableObject
{
    private readonly IReportRepository _reportRepository;
    private readonly User _currentUser;

    [ObservableProperty]
    private string _statusMessage = "";

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
        "Все", "Ожидает приговора", "В разработке", "Виновен (Принято)", "Оправдан (Отклонено)"
    };

    public AdminReportsViewModel(IReportRepository reportRepository, User user)
    {
        _reportRepository = reportRepository;
        _currentUser = user;
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

        if (_currentUser.Role?.Name == "Teacher" && SelectedReport.ViolatorId == _currentUser.Id)
        {
            StatusMessage = "Учитель не может изменять статус жалобы на самого себя!";
            return;
        }

        try
        {
            _reportRepository.UpdateStatus(SelectedReport.Id, statusStr);
            StatusMessage = $"Статус жалобы #{SelectedReport.Id} изменен на '{statusStr}'";
            LoadReports();
        }
        catch (Exception ex)
        {
            StatusMessage = "Ошибка: " + ex.Message;
        }
    }

    [RelayCommand]
    private void ExportToFile()
    {
        try
        {
            var lines = Reports.Select(r => 
                $"ID: {r.Id} | От: {(r.IsAnonymous ? "Аноним" : r.Author?.FullName)} | " +
                $"На: {r.Violator?.FullName} | Достоверность: {r.ReliabilityScore}% | " +
                $"Описание: {r.Description} | Статус: {r.Status?.Name}");
                
            System.IO.File.WriteAllLines("export_reports.txt", lines);
            StatusMessage = "Успешно экспортировано в export_reports.txt!";
        }
        catch (Exception ex)
        {
            StatusMessage = "Ошибка экспорта: " + ex.Message;
        }
    }
}
