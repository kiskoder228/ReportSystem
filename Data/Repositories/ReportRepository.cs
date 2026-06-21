using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ReportSystem.Models;

namespace ReportSystem.Data.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public ReportRepository(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public IEnumerable<Report> GetReportsByAuthor(int authorId)
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Reports
            .Include(r => r.Category)
            .Include(r => r.Status)
            .Include(r => r.Violator)
            .Where(r => r.AuthorId == authorId)
            .ToList();
    }

    public IEnumerable<Report> GetReportsByViolator(int violatorId)
    {
        using var db = _dbFactory.CreateDbContext();
        return db.Reports
            .Include(r => r.Category)
            .Include(r => r.Status)
            .Include(r => r.Author)
            .Where(r => r.ViolatorId == violatorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public IEnumerable<Report> GetAllReports(string? searchText, string? filterStatus)
    {
        using var db = _dbFactory.CreateDbContext();
        var query = db.Reports
            .Include(r => r.Status)
            .Include(r => r.Author)
            .Include(r => r.Category)
            .Include(r => r.Violator)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(r => r.Description.Contains(searchText));
        }

        if (!string.IsNullOrWhiteSpace(filterStatus) && filterStatus != "Все")
        {
            query = query.Where(r => r.Status != null && r.Status.Name == filterStatus);
        }

        return query.ToList();
    }

    public void AddReport(Report report)
    {
        using var db = _dbFactory.CreateDbContext();
        
        // Ensure status from DB is attached
        if (report.Status != null)
        {
            db.Entry(report.Status).State = EntityState.Unchanged;
        }
        else if (report.StatusId > 0)
        {
            var status = db.ReportStatuses.Find(report.StatusId);
            if (status != null)
            {
                report.Status = status;
            }
        }
        else
        {
            var statusNew = db.ReportStatuses.FirstOrDefault(s => s.Name == "Ожидает приговора");
            if (statusNew != null)
            {
                report.StatusId = statusNew.Id;
            }
        }

        db.Reports.Add(report);
        db.SaveChanges();
    }

    public void UpdateStatus(int reportId, string statusName)
    {
        using var db = _dbFactory.CreateDbContext();
        var report = db.Reports.Include(r => r.Author).Include(r => r.Violator).FirstOrDefault(r => r.Id == reportId);
        var status = db.ReportStatuses.FirstOrDefault(s => s.Name == statusName);
        if (report != null && status != null)
        {
            if (statusName == "Виновен (Принято)" && report.StatusId != status.Id)
            {
                if (report.Author != null) report.Author.Score += 10;
                if (report.Violator != null) report.Violator.Score -= 10;
            }

            report.StatusId = status.Id;
            if (statusName == "Виновен (Принято)" || statusName == "Оправдан (Отклонено)")
            {
                report.ResolvedAt = DateTime.UtcNow;
            }
            db.SaveChanges();
        }
    }

    public int GetTotalCount(int? authorId = null)
    {
        using var db = _dbFactory.CreateDbContext();
        var query = db.Reports.AsQueryable();
        if (authorId.HasValue)
        {
            query = query.Where(r => r.AuthorId == authorId.Value);
        }
        return query.Count();
    }

    public int GetCountByStatus(string statusName, int? authorId = null)
    {
        using var db = _dbFactory.CreateDbContext();
        var query = db.Reports.AsQueryable();
        if (authorId.HasValue)
        {
            query = query.Where(r => r.AuthorId == authorId.Value);
        }
        return query.Count(r => r.Status != null && r.Status.Name == statusName);
    }

    public int CalculateReliability(string description, bool isAnonymous)
    {
        int score = 50;
        if (isAnonymous) score -= 20;
        if (!string.IsNullOrWhiteSpace(description) && description.Length > 50) score += 15;
        
        var lowerDesc = description?.ToLower() ?? "";
        if (lowerDesc.Contains("точно") || lowerDesc.Contains("видел") || lowerDesc.Contains("свидетель"))
        {
            score += 35;
        }
        
        if (score > 100) score = 100;
        if (score < 0) score = 0;
        return score;
    }
}
