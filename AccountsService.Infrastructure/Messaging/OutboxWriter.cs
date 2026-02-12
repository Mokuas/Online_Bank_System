using AccountsService.Application.Messaging;
using AccountsService.Infrastructure.Data;
using System.Text.Json;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class OutboxWriter(AccountsDbContext db) : IOutboxWriter
    {
        public Task EnqueueAsync(string routingKey, object message, CancellationToken ct)
        {
            db.OutboxMessages.Add(new OutboxMessage
            {
                RoutingKey = routingKey,
                EventType = message.GetType().Name,
                PayloadJson = JsonSerializer.Serialize(message),
                OccurredAtUtc = DateTime.UtcNow
            });

            return Task.CompletedTask;
        }
    }
}
