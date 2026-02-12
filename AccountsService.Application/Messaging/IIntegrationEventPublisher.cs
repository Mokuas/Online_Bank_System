
namespace AccountsService.Application.Messaging
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent message, CancellationToken ct = default)
            where TEvent : notnull;
    }
}
