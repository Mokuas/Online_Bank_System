using AccountsService.Application.Dtos.Accounts;

namespace AccountsService.Application.Repositories
{
    public interface IAccountQueries
    {
        Task<IReadOnlyList<AccountResponse>> GetByCustomerIdAsync(int customerId, CancellationToken ct);
        Task<AccountResponse?> GetByIdAsync(int accountId, CancellationToken ct);
    }
}
