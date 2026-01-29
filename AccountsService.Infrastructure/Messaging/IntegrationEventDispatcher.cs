using System.Text.Json;
using AccountsService.Application.IntegrationEvents;
using AccountsService.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class IntegrationEventDispatcher(IServiceScopeFactory scopeFactory) : IIntegrationEventDispatcher
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public async Task DispatchAsync(string routingKey, string json, CancellationToken ct)
        {
            // One scope per message (DbContext + handlers are scoped)
            using var scope = scopeFactory.CreateScope();

            switch (routingKey)
            {
                case RoutingKeys.CustomerProfileCreated:
                    {
                        var message = JsonSerializer.Deserialize<CustomerProfileCreated>(json, JsonOptions)
                                      ?? throw new InvalidOperationException("Failed to deserialize CustomerProfileCreated.");

                        var handler = scope.ServiceProvider
                            .GetRequiredService<IIntegrationEventHandler<CustomerProfileCreated>>();

                        await handler.HandleAsync(message, ct);
                        return;
                    }

                default:
                    // Unknown message → ignore (or throw if you want strictness)
                    // For training + resilience, ignoring is OK.
                    return;
            }
        }
    }
}
