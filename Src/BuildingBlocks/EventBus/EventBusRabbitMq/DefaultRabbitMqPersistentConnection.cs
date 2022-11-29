using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBusRabbitMq;

public class DefaultRabbitMqPersistentConnection : IRabbitMqPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly int _retryCount;
    private IConnection _connection;
    public bool Disposed;

    private readonly object _syncRoot = new();

    public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _retryCount = retryCount;
    }

    public bool IsConnected => _connection is { IsOpen: true } && !Disposed;

    public IModel CreateModel()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");
        }

        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if (Disposed) return;

        Disposed = true;

        try
        {
            _connection.ConnectionShutdown -= OnConnectionShutdown;
            _connection.CallbackException -= OnCallbackException;
            _connection.ConnectionBlocked -= OnConnectionBlocked;
        }
        catch (IOException e)
        {

        }
    }

    public bool TryConnect()
    {
        lock (_syncRoot)
        {
            var policy = RetryPolicy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) =>
                    {

                    });

            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();
            });

            if (IsConnected)
            {
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;

                return true;
            }

            return false;
        }
    }

    private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
    {
        if (Disposed) return;
        
        TryConnect();
    }

    private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
    {
        if (Disposed) return;
        
        TryConnect();
    }

    private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
    {
        if (Disposed) return;
        
        TryConnect();
    }
}