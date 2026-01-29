using AccountsService.Application.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class RabbitMqEventBus : IEventBus, IAsyncDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly IConnection _connection;
        private readonly IChannel _channel;

        public RabbitMqEventBus(IOptions<RabbitMqOptions> options)
        {
            _options = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            // v7 async-first connection/channel (sync-bridged ONLY in ctor)
            _connection = factory
                .CreateConnectionAsync()
                .GetAwaiter()
                .GetResult();

            _channel = _connection
                .CreateChannelAsync()
                .GetAwaiter()
                .GetResult();

            _channel.ExchangeDeclareAsync(
                exchange: _options.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false)
                .GetAwaiter()
                .GetResult();
        }

        public async Task PublishAsync<T>(
            string routingKey,
            T message,
            CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await _channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: ct);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_channel is not null)
                    await _channel.CloseAsync();

                if (_connection is not null)
                    await _connection.CloseAsync();
            }
            catch
            {
                // ignore shutdown exceptions
            }
        }
    }
}
