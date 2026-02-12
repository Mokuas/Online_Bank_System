
namespace AccountsService.Application.Messaging
{
    public interface IOutboxWriter
    {
        Task EnqueueAsync(string routingKey, object message, CancellationToken ct);
    }
}
