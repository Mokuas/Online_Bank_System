using AccountsService.Application.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class RabbitMqEventBus : IEventBus
    {
        private readonly RabbitMqOptions _options;
        private readonly RabbitMqConnectionProvider _provider;

        public RabbitMqEventBus(
            RabbitMqConnectionProvider provider,
            IOptions<RabbitMqOptions> options)
        {
            _provider = provider;
            _options = options.Value;
        }

        public async Task PublishAsync<T>(string routingKey, T message, CancellationToken ct = default)
        {
            await _provider.InitializeAsync(_options, ct);

            var channel = _provider.Channel ?? throw new InvalidOperationException("RabbitMQ channel not initialized.");

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            await channel.BasicPublishAsync(
                exchange: _options.Exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: ct);
        }
    }
}
