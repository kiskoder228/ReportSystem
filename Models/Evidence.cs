using System;

namespace ReportSystem.Models;

public class Evidence
{
    public int Id { get; set; }

    public int ReportId { get; set; }
    public Report? Report { get; set; }

    public string FilePath { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
