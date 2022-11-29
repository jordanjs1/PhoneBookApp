using Autofac;
using EventBus;
using EventBus.Abstractions;
using EventBus.Events;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace EventBusRabbitMq;

public class EventBusRabbitMq : IEventBus, IDisposable
{
    public const string BrokerName = "phone_book_event_bus";
    public const string AutofacScopeName = "phone_book_event_bus";

    private readonly IRabbitMqPersistentConnection _persistentConnection;
    private readonly IEventBusSubscriptionsManager _subsManager;
    private readonly ILifetimeScope _autofac;
    private readonly int _retryCount;

    private IModel _consumerChannel;
    private string? _queueName;

    public EventBusRabbitMq(IRabbitMqPersistentConnection persistentConnection, ILifetimeScope autofac, IEventBusSubscriptionsManager subsManager, string? queueName = null, int retryCount = 5)
    {
        _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
        _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
        _queueName = queueName;
        _consumerChannel = CreateConsumerChannel();
        _autofac = autofac;
        _retryCount = retryCount;
        _subsManager.OnEventRemoved += SubsManager_OnEventRemoved;
    }

    private void SubsManager_OnEventRemoved(object sender, string eventName)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        using var channel = _persistentConnection.CreateModel();
        channel.QueueUnbind(
            queue: _queueName,
            exchange: BrokerName,
            routingKey: eventName);

        if (_subsManager.IsEmpty)
        {
            _queueName = string.Empty;
            _consumerChannel.Close();
        }
    }

    public void Publish(IntegrationEvent evt)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = RetryPolicy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
            {
                Console.WriteLine("Couldn't publish the event.");
            });

        var eventName = evt.GetType().Name;

        using var channel = _persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange: BrokerName, type: "direct");

        var body = JsonSerializer.SerializeToUtf8Bytes(evt, evt.GetType(), new JsonSerializerOptions
        {
            WriteIndented = true
        });

        policy.Execute(() =>
        {
            var properties = channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            channel.BasicPublish(
                exchange: BrokerName,
                routingKey: eventName,
                mandatory: true,
                basicProperties: properties,
                body: body);
        });
    }

    public void SubscribeDynamic<THandler>(string eventName)
        where THandler : IDynamicIntegrationEventHandler
    {
        DoInternalSubscription(eventName);
        _subsManager.AddDynamicSubscription<THandler>(eventName);
        StartBasicConsume();
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = _subsManager.GetEventKey<TEvent>();
        DoInternalSubscription(eventName);

        _subsManager.AddSubscription<TEvent, THandler>();
        StartBasicConsume();
    }

    private void DoInternalSubscription(string eventName)
    {
        var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
        if (containsKey) return;

        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        _consumerChannel.QueueBind(
            queue: _queueName,
            exchange: BrokerName,
            routingKey: eventName);
    }

    public void Unsubscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>
    {
        var eventName = _subsManager.GetEventKey<TEvent>();

        _subsManager.RemoveSubscription<TEvent, THandler>();
    }

    public void UnsubscribeDynamic<TH>(string eventName)
        where TH : IDynamicIntegrationEventHandler
    {
        _subsManager.RemoveDynamicSubscription<TH>(eventName);
    }

    public void Dispose()
    {
        if (_consumerChannel != null)
        {
            _consumerChannel.Dispose();
        }

        _subsManager.Clear();
    }

    private void StartBasicConsume()
    {
        if (_consumerChannel != null)
        {
            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += Consumer_Received;

            _consumerChannel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }
        else
        {
            Console.WriteLine("Consumer channel is null.");
        }
    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            await ProcessEventAsync(eventName, message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while processing message: {e.Message}");
            if (e.InnerException != null)
                Console.WriteLine($"Inner exception: {e.InnerException.Message}");

            Console.WriteLine(e.StackTrace);
        }

        _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var channel = _persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange: BrokerName,
            type: "direct");

        channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.CallbackException += (sender, ea) =>
        {
            _consumerChannel.Dispose();
            _consumerChannel = CreateConsumerChannel();
            StartBasicConsume();
        };

        return channel;
    }

    private async Task ProcessEventAsync(string eventName, string message)
    {
        if (_subsManager.HasSubscriptionsForEvent(eventName))
        {
            await using var scope = _autofac.BeginLifetimeScope(AutofacScopeName);
            var subscriptions = _subsManager.GetHandlersForEvent(eventName);

            foreach (var subscription in subscriptions)
            {
                if (subscription.IsDynamic)
                {
                    if (scope.ResolveOptional(subscription.HandlerType) is not IDynamicIntegrationEventHandler handler) 
                        continue;

                    using dynamic eventData = JsonDocument.Parse(message);
                    await Task.Yield();
                    await handler.HandleAsync(eventData);
                }
                else
                {
                    var handler = scope.ResolveOptional(subscription.HandlerType);
                    if (handler == null)
                        continue;

                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await ((Task)concreteType.GetMethod("HandleAsync")!.Invoke(handler, new[] { integrationEvent })!)!;
                }
            }
        }
        else
        {
            Console.WriteLine("No subscription for the event found.");
        }
    }
}