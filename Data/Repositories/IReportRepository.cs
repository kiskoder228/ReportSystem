using System.Collections.Generic;
using ReportSystem.Models;

namespace ReportSystem.Data.Repositories;

public interface IReportRepository
{
    IEnumerable<Report> GetReportsByAuthor(int authorId);
    IEnumerable<Report> GetAllReports(string? searchText, string? filterStatus);
    void AddReport(Report report);
    void UpdateStatus(int reportId, string statusName);
    int GetTotalCount(int? authorId = null);
    int GetCountByStatus(string statusName, int? authorId = null);
    int CalculateReliability(string description, bool isAnonymous);
}
