using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data.Repositories;
using ReportSystem.Models;

namespace ReportSystem.ViewModels;

public partial class AdminUsersViewModel : ObservableObject
{
    private readonly IUserRepository _userRepository;

    [ObservableProperty]
    private User? _selectedUser;

    [ObservableProperty]
    private string _searchText = "";

    partial void OnSearchTextChanged(string value)
    {
        LoadUsers();
    }

    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<string> Roles { get; } = new() { "Student", "Teacher", "Admin" };

    public AdminUsersViewModel(IUserRepository userRepository, User currentAdmin)
    {
        _userRepository = userRepository;
        LoadUsers();
    }

    private void LoadUsers()
    {
        Users.Clear();
        try
        {
            var query = _userRepository.GetAllUsers(SearchText);
            foreach (var user in query)
                Users.Add(user);
        }
        catch { }
    }

    [RelayCommand]
    private void ChangeRole(string? newRole)
    {
        if (SelectedUser == null || string.IsNullOrEmpty(newRole)) return;

        try
        {
            _userRepository.UpdateRole(SelectedUser.Id, newRole);
            LoadUsers();
        }
        catch { }
    }

    [RelayCommand]
    private void DeleteUser()
    {
        if (SelectedUser == null) return;

        try
        {
            _userRepository.DeleteUser(SelectedUser.Id);
            LoadUsers();
        }
        catch { }
    }
}
