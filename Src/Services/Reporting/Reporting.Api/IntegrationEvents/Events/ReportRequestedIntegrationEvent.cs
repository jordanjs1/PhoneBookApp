using EventBus.Events;

namespace Reporting.Api.IntegrationEvents.Events;

public record ReportRequestedIntegrationEvent : IntegrationEvent
{
    public Guid ReportId { get; set; }
}