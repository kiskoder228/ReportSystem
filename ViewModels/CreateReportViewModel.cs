using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public class CreateReportViewModel : ObservableObject
{
    private readonly User _currentUser;
    private Category? _selectedCategory;
    private string _description = "";
    private bool _isAnonymous;
    private string _statusMessage = "";
    private bool _hasMessage;
    private bool _isSuccess;

    public ObservableCollection<Category> Categories { get; } = new();

    public Category? SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsAnonymous
    {
        get => _isAnonymous;
        set => SetProperty(ref _isAnonymous, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool HasMessage
    {
        get => _hasMessage;
        set => SetProperty(ref _hasMessage, value);
    }

    public bool IsSuccess
    {
        get => _isSuccess;
        set => SetProperty(ref _isSuccess, value);
    }

    public IRelayCommand SubmitCommand { get; }
    public IRelayCommand ClearCommand { get; }

    public CreateReportViewModel(User user)
    {
        _currentUser = user;
        SubmitCommand = new RelayCommand(SubmitReport);
        ClearCommand = new RelayCommand(Clear);
        LoadCategories();
    }

    private void LoadCategories()
    {
        try
        {
            using var db = new ApplicationDbContext();
            foreach (var cat in db.Categories.ToList())
                Categories.Add(cat);

            SelectedCategory = Categories.FirstOrDefault();
        }
        catch { }
    }

    private void ShowMessage(bool success, string message)
    {
        IsSuccess = success;
        StatusMessage = message;
        HasMessage = true;
    }

    private void SubmitReport()
    {
        if (SelectedCategory == null || string.IsNullOrWhiteSpace(Description))
        {
            ShowMessage(false, "Заполните описание и выберите категорию!");
            return;
        }

        try
        {
            using var db = new ApplicationDbContext();

            var statusNew = db.ReportStatuses.FirstOrDefault(s => s.Name == "New");

            var report = new Report
            {
                AuthorId = _currentUser.Id,
                CategoryId = SelectedCategory.Id,
                Description = Description,
                IsAnonymous = IsAnonymous,
                Status = statusNew,
                CreatedAt = DateTime.UtcNow
            };

            db.Reports.Add(report);
            db.SaveChanges();

            ShowMessage(true, "✅ Обращение успешно отправлено!");
            Clear();
        }
        catch (Exception ex)
        {
            ShowMessage(false, "Ошибка: " + ex.Message);
        }
    }

    private void Clear()
    {
        Description = "";
        IsAnonymous = false;
        SelectedCategory = Categories.FirstOrDefault();
    }
}
