
namespace AccountsService.Infrastructure.Messaging
{
    public interface IIntegrationEventRouting
    {
        string GetRoutingKey(object message);
    }
}
