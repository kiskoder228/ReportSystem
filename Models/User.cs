using System;
using System.Collections.Generic;

namespace ReportSystem.Models;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Report> Reports { get; set; } = new();
    public List<Report> ReviewedReports { get; set; } = new();
}