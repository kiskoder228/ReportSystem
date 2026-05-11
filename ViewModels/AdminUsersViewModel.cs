using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReportSystem.Data;
using ReportSystem.Models;

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
                var allUsers = db.Users.ToList();
                
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
                var user = db.Users.FirstOrDefault(u => u.Id == SelectedUser.Id);
                if (user != null)
                {
                    user.Role = newRole;
                    db.SaveChanges();
                    
                    SelectedUser.Role = newRole;
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
