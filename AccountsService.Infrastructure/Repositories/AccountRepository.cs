using AccountsService.Application.Repositories;
using AccountsService.Domain.Entities;
using AccountsService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Repositories
{
    public sealed class AccountRepository(AccountsDbContext db) : IAccountRepository
    {

        public Task<Account?> GetByIdAsync(int id)
            => db.Accounts.FirstOrDefaultAsync(a => a.Id == id);

        public Task<Account?> GetByIdReadAsync(int id)
            => db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        public async Task<IReadOnlyList<Account>> GetByCustomerIdAsync(int customerId)
            => await db.Accounts.AsNoTracking()
                .Where(a => a.CustomerId == customerId)
                .OrderBy(a => a.Id)
                .ToListAsync();

        public async Task AddAsync(Account account)
            => await db.Accounts.AddAsync(account);

        public Task SaveChangesAsync()
            => db.SaveChangesAsync();

        public Task<bool> AccountNumberExistsAsync(string accountNumber)
            => db.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);
    }
}
