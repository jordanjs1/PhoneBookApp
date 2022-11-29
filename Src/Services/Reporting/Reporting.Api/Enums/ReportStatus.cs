namespace Reporting.Api.Enums;

/// <summary>
/// Defines possible report states.
/// </summary>
public enum ReportStatus
{
    /// <summary>
    /// Indicates that the report is being processed.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Indicates that the report is ready to be viewed.
    /// </summary>
    Ready = 2,

    /// <summary>
    /// Indicates that the report generation is failed.
    /// </summary>
    Failed = 3
}