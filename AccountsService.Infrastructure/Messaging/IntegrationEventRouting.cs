using AccountsService.Application.IntegrationEvents;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class IntegrationEventRouting : IIntegrationEventRouting
    {
        public string GetRoutingKey(object message) =>
            message switch
            {
                AccountCreated => IntegrationRoutingKeys.AccountCreated,
                AccountStatusChanged => IntegrationRoutingKeys.AccountStatusChanged,
                _ => throw new UnknownRoutingKeyException(message.GetType().Name)
            };
    }
}
