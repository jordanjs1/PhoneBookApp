using EventBus.Abstractions;
using Reporting.Api.IntegrationEvents.Events;
using Reporting.Api.Services;

namespace Reporting.Api.IntegrationEvents.EventHandling
{
    public class ReportRequestedIntegrationEventHandler : IIntegrationEventHandler<ReportRequestedIntegrationEvent>
    {
        private readonly IReportingService _reportingService;

        public ReportRequestedIntegrationEventHandler(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        public async Task HandleAsync(ReportRequestedIntegrationEvent evt)
        {
            await _reportingService.DoReportGenerationAsync(evt.ReportId);
        }
    }
}
