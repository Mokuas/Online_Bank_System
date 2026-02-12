
namespace AccountsService.Application.Common
{
    public interface ITransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken ct);
    }
}
