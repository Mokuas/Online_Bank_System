using System.Text;
using System.Text.Json;
using AccountsService.Application.Messaging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AccountsService.Infrastructure.Messaging
{
    internal sealed class CustomerProfileConsumer : AsyncDefaultBasicConsumer
    {
        private readonly IIntegrationEventDispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly CancellationToken _stoppingToken;

        public CustomerProfileConsumer(
            IChannel channel,
            IIntegrationEventDispatcher dispatcher,
            ILogger logger,
            CancellationToken stoppingToken)
            : base(channel)
        {
            _dispatcher = dispatcher;
            _logger = logger;
            _stoppingToken = stoppingToken;
        }

        public override async Task HandleBasicDeliverAsync(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IReadOnlyBasicProperties properties,
            ReadOnlyMemory<byte> body,
            CancellationToken cancellationToken)
        {
            if (_stoppingToken.IsCancellationRequested)
                return;

            string json = string.Empty;
            try
            {
                json = Encoding.UTF8.GetString(body.Span);
                await _dispatcher.DispatchAsync(routingKey, json, _stoppingToken);

                await Channel.BasicAckAsync(deliveryTag, multiple: false, _stoppingToken);
            }
            catch (OperationCanceledException) when (_stoppingToken.IsCancellationRequested || cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error handling RabbitMQ message. RoutingKey={RoutingKey}",
                    routingKey);

                var poison = IsPoisonMessage(ex);

                _logger.LogError(ex,
                    "Error handling RabbitMQ message. RoutingKey={RoutingKey} Poison={Poison} Redelivered={Redelivered}",
                    routingKey,
                    poison,
                    redelivered);

                await Channel.BasicNackAsync(
                    deliveryTag,
                    multiple: false,
                    requeue: !poison,
                    _stoppingToken);
            }
        }

        private static bool IsPoisonMessage(Exception ex)
       {
          // Non-transient parsing/contract issues should not be retried forever.
          if (ex is JsonException or NotSupportedException or FormatException)
              return true;

          if (ex is UnknownRoutingKeyException)
              return true;

            // dispatcher throws InvalidOperationException when deserialization fails.
            // Treat that as poison too.
          if (ex is InvalidOperationException)
            return true;

          return false; // default: assume transient
      }
}
}
