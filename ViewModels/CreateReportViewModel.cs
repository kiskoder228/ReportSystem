using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class CreateReportViewModel : ObservableObject
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly User _currentUser;

    [ObservableProperty]
    private Category? _selectedCategory;

    [ObservableProperty]
    private User? _selectedViolator;

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private bool _isAnonymous;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _hasMessage;

    [ObservableProperty]
    private bool _isSuccess;

    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<User> AvailableViolators { get; } = new();

    public CreateReportViewModel(
        ICategoryRepository categoryRepository,
        IReportRepository reportRepository,
        IUserRepository userRepository,
        User user)
    {
        _categoryRepository = categoryRepository;
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _currentUser = user;
        LoadCategories();
        LoadViolators();
    }

    private void LoadViolators()
    {
        try
        {
            AvailableViolators.Clear();
            foreach (var v in _userRepository.GetAvailableViolators(_currentUser.Id))
                AvailableViolators.Add(v);
        }
        catch { }
    }

    private void LoadCategories()
    {
        try
        {
            foreach (var cat in _categoryRepository.GetAll())
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

    [RelayCommand]
    private void Submit()
    {
        if (SelectedCategory == null || SelectedViolator == null || string.IsNullOrWhiteSpace(Description))
        {
            ShowMessage(false, "Заполните описание, выберите нарушителя и категорию!");
            return;
        }

        try
        {
            var relScore = _reportRepository.CalculateReliability(Description, IsAnonymous);

            var report = new Report
            {
                AuthorId = _currentUser.Id,
                CategoryId = SelectedCategory.Id,
                ViolatorId = SelectedViolator.Id,
                ReliabilityScore = relScore,
                Description = Description,
                IsAnonymous = IsAnonymous,
                CreatedAt = DateTime.UtcNow
            };

            _reportRepository.AddReport(report);

            ShowMessage(true, "✅ Обращение успешно отправлено!");
            Clear();
        }
        catch (Exception ex)
        {
            ShowMessage(false, "Ошибка: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Description = "";
        IsAnonymous = false;
        SelectedCategory = Categories.FirstOrDefault();
        SelectedViolator = null;
    }
}
