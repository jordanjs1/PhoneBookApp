using EventBus.Events;

namespace EventBus.Abstractions;

public interface IEventBus
{
    void Publish(IntegrationEvent evt);

    void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;

    void SubscribeDynamic<THandler>(string eventName)
        where THandler : IDynamicIntegrationEventHandler;

    void Unsubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;

    void UnsubscribeDynamic<THandler>(string eventName)
        where THandler : IDynamicIntegrationEventHandler;
}