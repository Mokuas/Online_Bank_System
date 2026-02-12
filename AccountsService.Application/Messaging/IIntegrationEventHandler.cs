
namespace AccountsService.Application.Messaging
{
    public interface IIntegrationEventHandler<in T>
    {
        Task HandleAsync(T message, CancellationToken ct);
    }
}
