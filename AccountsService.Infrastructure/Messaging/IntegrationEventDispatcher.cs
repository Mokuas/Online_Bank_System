using AccountsService.Application.IntegrationEvents;
using AccountsService.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class IntegrationEventDispatcher(
        IServiceScopeFactory scopeFactory,
        ILogger<IntegrationEventDispatcher> logger) : IIntegrationEventDispatcher
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public async Task DispatchAsync(string routingKey, string json, CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();

            switch (routingKey)
            {
                case Messaging.RoutingKeys.CustomerProfileCreated:
                    {
                        var message = JsonSerializer.Deserialize<CustomerProfileCreated>(json, JsonOptions)
                                      ?? throw new InvalidOperationException("Failed to deserialize CustomerProfileCreated.");

                        var handler = scope.ServiceProvider
                            .GetRequiredService<IIntegrationEventHandler<CustomerProfileCreated>>();

                        await handler.HandleAsync(message, ct);
                        return;
                    }

                default:
                    logger.LogError(
                        "Unknown routing key received. RoutingKey={RoutingKey} Payload={Payload}",
                        routingKey,
                        json);

                    throw new UnknownRoutingKeyException(routingKey);
            }
        }
    }
}
