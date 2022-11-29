using EventBus.Abstractions;
using EventBus.Events;

namespace EventBus;

public partial class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    private readonly Dictionary<string, List<SubscriptionInformation>> _handlers;
    private readonly List<Type> _eventTypes;

    public event EventHandler<string> OnEventRemoved;

    public InMemoryEventBusSubscriptionsManager()
    {
        _handlers = new Dictionary<string, List<SubscriptionInformation>>();
        _eventTypes = new List<Type>();
    }

    public bool IsEmpty => _handlers is { Count: 0 };
    public void Clear() => _handlers.Clear();

    public void AddDynamicSubscription<THandler>(string eventName)
        where THandler : IDynamicIntegrationEventHandler
    {
        DoAddSubscription(typeof(THandler), eventName, isDynamic: true);
    }

    public void AddSubscription<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = GetEventKey<TEvent>();

        DoAddSubscription(typeof(THandler), eventName, isDynamic: false);

        if (!_eventTypes.Contains(typeof(TEvent)))
        {
            _eventTypes.Add(typeof(TEvent));
        }
    }

    private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
    {
        if (!HasSubscriptionsForEvent(eventName))
        {
            _handlers.Add(eventName, new List<SubscriptionInformation>());
        }

        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
        {
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
        }

        _handlers[eventName].Add(isDynamic
            ? SubscriptionInformation.Dynamic(handlerType)
            : SubscriptionInformation.Typed(handlerType));
    }


    public void RemoveDynamicSubscription<THandler>(string eventName)
        where THandler : IDynamicIntegrationEventHandler
    {
        var handlerToRemove = FindDynamicSubscriptionToRemove<THandler>(eventName);
        DoRemoveHandler(eventName, handlerToRemove);
    }


    public void RemoveSubscription<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var handlerToRemove = FindSubscriptionToRemove<TEvent, THandler>();
        var eventName = GetEventKey<TEvent>();
        DoRemoveHandler(eventName, handlerToRemove);
    }


    private void DoRemoveHandler(string eventName, SubscriptionInformation subsToRemove)
    {
        if (subsToRemove != null)
        {
            _handlers[eventName].Remove(subsToRemove);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null)
                {
                    _eventTypes.Remove(eventType);
                }
                RaiseOnEventRemoved(eventName);
            }
        }
    }

    public IEnumerable<SubscriptionInformation> GetHandlersForEvent<TEvent>() where TEvent : IntegrationEvent
    {
        var key = GetEventKey<TEvent>();
        return GetHandlersForEvent(key);
    }
    public IEnumerable<SubscriptionInformation> GetHandlersForEvent(string eventName) => _handlers[eventName];

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }


    private SubscriptionInformation FindDynamicSubscriptionToRemove<THandler>(string eventName)
        where THandler : IDynamicIntegrationEventHandler
    {
        return DoFindSubscriptionToRemove(eventName, typeof(THandler));
    }


    private SubscriptionInformation FindSubscriptionToRemove<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = GetEventKey<TEvent>();
        return DoFindSubscriptionToRemove(eventName, typeof(THandler));
    }

    private SubscriptionInformation DoFindSubscriptionToRemove(string eventName, Type handlerType)
    {
        return !HasSubscriptionsForEvent(eventName) 
            ? null 
            : _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
    }

    public bool HasSubscriptionsForEvent<TEvent>() where TEvent : IntegrationEvent
    {
        var key = GetEventKey<TEvent>();
        return HasSubscriptionsForEvent(key);
    }
    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

    public string GetEventKey<T>()
    {
        return typeof(T).Name;
    }
}
