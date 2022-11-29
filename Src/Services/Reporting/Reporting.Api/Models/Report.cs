using Reporting.Api.Enums;

namespace Reporting.Api.Models;

public class Report : EntityBase
{
    public DateTime RequestDate { get; set; } = DateTime.Now;

    public ReportStatus Status { get; set; } = ReportStatus.Processing;

    public string? PathToReportFile { get; set; } = null;
}