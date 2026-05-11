using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data;
using ReportSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ReportSystem.ViewModels;

public class AdminUsersViewModel : ObservableObject
{
    private User? _selectedUser;
    private string _searchText = "";

    public ObservableCollection<User> Users { get; } = new();
    public ObservableCollection<string> Roles { get; } = new() { "Student", "Teacher", "Admin" };

    public User? SelectedUser
    {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                LoadUsers();
            }
        }
    }

    public IRelayCommand<string> ChangeRoleCommand { get; }

    public AdminUsersViewModel(User currentAdmin)
    {
        ChangeRoleCommand = new RelayCommand<string>(ChangeRole);
        LoadUsers();
    }

    private void LoadUsers()
    {
        Users.Clear();
        try
        {
            using (var db = new ApplicationDbContext())
            {
                // Загружаем всех пользователей
                var allUsers = db.Users.Include(u => u.Role).ToList();
                
                foreach (var user in allUsers)
                {
                    // Простая фильтрация в памяти
                    if (!string.IsNullOrWhiteSpace(SearchText))
                    {
                        bool matches = (user.FullName != null && user.FullName.ToLower().Contains(SearchText.ToLower())) || 
                                       (user.Login != null && user.Login.ToLower().Contains(SearchText.ToLower()));
                        if (!matches) continue;
                    }

                    Users.Add(user);
                }
            }
        }
        catch (Exception)
        {
            // ошибка
        }
    }

    private void ChangeRole(string? newRole)
    {
        if (SelectedUser == null || string.IsNullOrEmpty(newRole)) return;

        try
        {
            using (var db = new ApplicationDbContext())
            {
                var role = db.Roles.FirstOrDefault(r => r.Name == newRole);
                if (role == null) return;

                var user = db.Users.FirstOrDefault(u => u.Id == SelectedUser.Id);
                if (user != null)
                {
                    user.RoleId = role.Id;
                    db.SaveChanges();
                    
                    SelectedUser.Role = role;
                    LoadUsers(); 
                }
            }
        }
        catch (Exception)
        {
            // ошибка
        }
    }
}
