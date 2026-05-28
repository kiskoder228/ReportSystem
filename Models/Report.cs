using System;
using System.Collections.Generic;

namespace ReportSystem.Models;

public class Report
{
    public int Id { get; set; }

    public int AuthorId { get; set; }
    public User? Author { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string Description { get; set; } = null!;

    public int StatusId { get; set; }
    public ReportStatus? Status { get; set; }
    public bool IsAnonymous { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? ReviewedById { get; set; }
    public User? ReviewedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? ModeratorComment { get; set; }

    public List<Evidence> Evidences { get; set; } = new();
}
