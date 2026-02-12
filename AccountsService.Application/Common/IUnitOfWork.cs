
namespace AccountsService.Application.Common
{
    public interface IUnitOfWork
    {
        Task<ITransaction> BeginTransactionAsync(CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
