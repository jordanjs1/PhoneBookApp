namespace EventBus.Abstractions;

public interface IIntegrationEventHandler { }

public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
{
    Task HandleAsync(TIntegrationEvent evt);
}