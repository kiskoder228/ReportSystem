using System;
using System.Collections.Generic;

namespace ReportSystem.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "Student";
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public List<Report> Reports { get; set; } = new();
    public List<Report> ReviewedReports { get; set; } = new();
}