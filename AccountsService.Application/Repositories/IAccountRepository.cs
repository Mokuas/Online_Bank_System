using AccountsService.Domain.Entities;
using AccountsService.Domain.Enums;

namespace AccountsService.Application.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetByIdReadAsync(int id);

        Task<IReadOnlyList<Account>> GetByCustomerIdAsync(int customerId);

        Task AddAsync(Account account);
        Task SaveChangesAsync();

        Task<bool> AccountNumberExistsAsync(string accountNumber);
    }
}
