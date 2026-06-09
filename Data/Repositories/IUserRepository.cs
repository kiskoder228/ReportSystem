using System.Collections.Generic;
using ReportSystem.Models;

namespace ReportSystem.Data.Repositories;

public interface IUserRepository
{
    User? ValidateUser(string login, string password);
    User RegisterUser(string fullName, string login, string password);
    IEnumerable<User> GetAllUsers(string? searchText);
    void UpdateRole(int userId, string roleName);
    bool UserExists(string login);
    void DeleteUser(int userId);
}
