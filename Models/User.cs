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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Address { get; set; } = string.Empty;
    public int Score { get; set; } = 0;
    
    public string Rank 
    { 
        get 
        {
            if (Score < 0) return "Мученик";
            if (Score <= 20) return "Подлиза";
            if (Score <= 50) return "Смотрящий";
            if (Score <= 90) return "Стукач";
            return "Крыса";
        }
    }

    public List<Report> Reports { get; set; } = new();
    public List<Report> ReviewedReports { get; set; } = new();
    public List<Report> Violations { get; set; } = new();
}