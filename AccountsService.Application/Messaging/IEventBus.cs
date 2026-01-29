
namespace AccountsService.Application.Messaging
{
    public interface IEventBus
    {
        Task PublishAsync<T>(
            string routingKey,
            T message,
            CancellationToken ct = default);
    }
}
