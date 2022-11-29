using EventBus.Abstractions;
using EventBus.Events;
using Reporting.Api.Infrastructure;

namespace Reporting.Api.IntegrationEvents
{
    public class ReportingIntegrationEventService : IReportingIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly ReportingContext _reportingContext;
        private volatile bool _disposedValue;

        public ReportingIntegrationEventService(IEventBus eventBus, ReportingContext reportingContext)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _reportingContext = reportingContext ?? throw new ArgumentNullException(nameof(reportingContext));
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            _eventBus.Publish(evt);
        }
    }
}
