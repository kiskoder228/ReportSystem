using System;
using System.Collections.Generic;
using ReportSystem.Models.Enums;

namespace ReportSystem.Models;

public class Report
{
    public int Id { get; set; }

    // Автор сообщения
    public int AuthorId { get; set; }
    public User? Author { get; set; }

    // Категория нарушения
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string Description { get; set; } = null!;

    public int StatusId { get; set; }
    public ReportStatus Status { get; set; }
    // Анонимная подача
    public bool IsAnonymous { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Кто рассматривал (ID администратора)
    public int? ReviewedById { get; set; }
    public User? ReviewedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    // Комментарий модератора
    public string? ModeratorComment { get; set; }

    // Доказательства
    public List<Evidence> Evidences { get; set; } = new();
}
