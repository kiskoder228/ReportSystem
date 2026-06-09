using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Models;
using BC = BCrypt.Net.BCrypt;

namespace ReportSystem.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public UserRepository(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public User? ValidateUser(string login, string password)
    {
        using var db = _dbFactory.CreateDbContext();
        var user = db.Users.Include(u => u.Role).FirstOrDefault(u => u.Login == login);
        if (user != null && BC.Verify(password, user.PasswordHash))
        {
            return user;
        }
        return null;
    }

    public User RegisterUser(string fullName, string login, string password)
    {
        using var db = _dbFactory.CreateDbContext();
        var role = db.Roles.FirstOrDefault(r => r.Name == "Student");
        
        var newUser = new User
        {
            FullName = fullName,
            Login = login,
            PasswordHash = BC.HashPassword(password),
            Role = role,
            RoleId = role?.Id ?? 3,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(newUser);
        db.SaveChanges();
        return newUser;
    }

    public IEnumerable<User> GetAllUsers(string? searchText)
    {
        using var db = _dbFactory.CreateDbContext();
        var query = db.Users.Include(u => u.Role).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(u =>
                u.FullName.Contains(searchText) ||
                u.Login.Contains(searchText));
        }

        return query.ToList();
    }

    public void UpdateRole(int userId, string roleName)
    {
        using var db = _dbFactory.CreateDbContext();
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        var role = db.Roles.FirstOrDefault(r => r.Name == roleName);
        if (user != null && role != null)
        {
            user.RoleId = role.Id;
            db.SaveChanges();
        }
    }

    public bool UserExists(string login)
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Users.Any(u => u.Login == login);
    }

    public void DeleteUser(int userId)
    {
        using var db = _dbFactory.CreateDbContext();
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            db.Users.Remove(user);
            db.SaveChanges();
        }
    }
}
