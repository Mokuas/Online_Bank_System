using AccountsService.Application.Messaging;
using Microsoft.Extensions.Logging;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class IntegrationEventPublisher(
         IOutboxWriter outbox,
         IIntegrationEventRouting routing,
         ILogger<IntegrationEventPublisher> Logger) : IIntegrationEventPublisher
    {
        public Task PublishAsync<TEvent>(TEvent message, CancellationToken ct = default)
            where TEvent : notnull
        {
            string routingKey;
            try
            {
                routingKey = routing.GetRoutingKey(message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "No routing key mapping for integration event type {EventType}", typeof(TEvent).FullName);
                throw;
            }
            return outbox.EnqueueAsync(routingKey, message, ct);
        }
    }
}
