using AccountsService.Domain.Entities;
using AccountsService.Domain.Enums;

namespace AccountsService.Application.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int id, CancellationToken ct);
        Task<Account?> GetByIdReadAsync(int id, CancellationToken ct);

        Task<IReadOnlyList<Account>> GetByCustomerIdAsync(int customerId, CancellationToken ct);

        Task AddAsync(Account account, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);

        Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken ct);
    }
}
