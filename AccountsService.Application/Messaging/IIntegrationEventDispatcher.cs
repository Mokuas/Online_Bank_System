
namespace AccountsService.Application.Messaging
{
    public interface IIntegrationEventDispatcher
    {
        Task DispatchAsync(string routingKey, string json, CancellationToken ct);
    }
}
