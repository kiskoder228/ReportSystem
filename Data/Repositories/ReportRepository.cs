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
            .Where(r => r.AuthorId == authorId)
            .ToList();
    }

    public IEnumerable<Report> GetAllReports(string? searchText, string? filterStatus)
    {
        using var db = _dbFactory.CreateDbContext();
        var query = db.Reports
            .Include(r => r.Status)
            .Include(r => r.Author)
            .Include(r => r.Category)
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
            var statusNew = db.ReportStatuses.FirstOrDefault(s => s.Name == "New");
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
        var report = db.Reports.FirstOrDefault(r => r.Id == reportId);
        var status = db.ReportStatuses.FirstOrDefault(s => s.Name == statusName);
        if (report != null && status != null)
        {
            report.StatusId = status.Id;
            if (statusName == "Resolved" || statusName == "Rejected")
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
}
