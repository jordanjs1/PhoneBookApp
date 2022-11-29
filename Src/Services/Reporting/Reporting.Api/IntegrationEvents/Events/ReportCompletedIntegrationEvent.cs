using EventBus.Events;
using Reporting.Api.Enums;

namespace Reporting.Api.IntegrationEvents.Events
{
    public record ReportCompletedIntegrationEvent : IntegrationEvent
    {
        public Guid ReportId { get; set; }

        public ReportStatus Status { get; set; }

        public string? PathToReportFile { get; set; }
    }
}
