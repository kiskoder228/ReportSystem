using System.Collections.Generic;

namespace ReportSystem.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int SeverityLevel { get; set; } = 1; // 1 - низкий, 2 - средний, 3 - высокий

    // Навигационное свойство
    public List<Report> Reports { get; set; } = new();
}
