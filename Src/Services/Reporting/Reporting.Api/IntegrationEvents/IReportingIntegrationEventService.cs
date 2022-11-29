using EventBus.Events;

namespace Reporting.Api.IntegrationEvents
{
    public interface IReportingIntegrationEventService
    {

        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
