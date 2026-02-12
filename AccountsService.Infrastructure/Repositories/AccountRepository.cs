using AccountsService.Application.Repositories;
using AccountsService.Domain.Entities;
using AccountsService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Repositories
{
    public sealed class AccountRepository(AccountsDbContext db) : IAccountRepository
    {

        public Task<Account?> GetByIdAsync(int id, CancellationToken ct)
            => db.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

        public Task<Account?> GetByIdReadAsync(int id, CancellationToken ct)
            => db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);

        public async Task<IReadOnlyList<Account>> GetByCustomerIdAsync(int customerId, CancellationToken ct)
            => await db.Accounts.AsNoTracking()
                .Where(a => a.CustomerId == customerId)
                .OrderBy(a => a.Id)
                .ToListAsync(ct);

        public async Task AddAsync(Account account, CancellationToken ct)
            => await db.Accounts.AddAsync(account, ct);

        public Task SaveChangesAsync(CancellationToken ct)
            => db.SaveChangesAsync(ct);

        public Task<bool> AccountNumberExistsAsync(string accountNumber, CancellationToken ct)
            => db.Accounts.AsNoTracking().AnyAsync(a => a.AccountNumber == accountNumber, ct);
    }
}
