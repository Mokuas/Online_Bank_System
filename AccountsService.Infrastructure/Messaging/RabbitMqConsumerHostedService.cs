using System.Text;
using AccountsService.Application.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class RabbitMqConsumerHostedService(IOptions<RabbitMqOptions> options, IIntegrationEventDispatcher dispatcher, ILogger<RabbitMqConsumerHostedService> logger) : BackgroundService
    {
        private readonly RabbitMqOptions _options = options.Value;
        private IConnection? _connection;
        private IChannel? _channel;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.Host,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(
                _options.Exchange,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                _options.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                _options.Queue,
                _options.Exchange,
                RoutingKeys.CustomerProfileCreated,
                cancellationToken: stoppingToken);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 16,
                global: false,
                cancellationToken: stoppingToken);

            var consumer = new CustomerProfileConsumer(
                _channel,
                dispatcher,
                logger,
                stoppingToken);

            await _channel.BasicConsumeAsync(
                queue: _options.Queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            logger.LogInformation(
                "RabbitMQ consumer started. Exchange={Exchange}, Queue={Queue}",
                _options.Exchange,
                _options.Queue);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_channel is not null)
                    await _channel.CloseAsync(cancellationToken);

                if (_connection is not null)
                    await _connection.CloseAsync(cancellationToken);
            }
            catch
            {
                // ignore shutdown exceptions
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
