using EventBus.Events;

namespace Contact.Api.IntegrationEvents
{
    public interface IBookIntegrationEventService
    {
        Task SaveEventAndBookContextChangesAsync(IntegrationEvent evt);
        Task PublishThroughEventBusAsync(IntegrationEvent evt);
    }
}
