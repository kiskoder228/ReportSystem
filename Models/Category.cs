using System.Collections.Generic;

namespace ReportSystem.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int SeverityLevel { get; set; } = 1;

    public List<Report> Reports { get; set; } = new();
}
