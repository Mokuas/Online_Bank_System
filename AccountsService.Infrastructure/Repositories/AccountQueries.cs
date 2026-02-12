using AccountsService.Application.Dtos.Accounts;
using AccountsService.Application.Repositories;
using AccountsService.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Repositories
{
    public sealed class AccountQueries(AccountsDbContext db) : IAccountQueries
    {
        public async Task<IReadOnlyList<AccountResponse>> GetByCustomerIdAsync(int customerId, CancellationToken ct)
        {
            var items = await db.Accounts
                .AsNoTracking()
                .Where(a => a.CustomerId == customerId)
                .ProjectToType<AccountResponse>()
                .ToListAsync(ct);

            return items.AsReadOnly();
        }

        public async Task<AccountResponse?> GetByIdAsync(int accountId, CancellationToken ct)
        {
            return await db.Accounts
                .AsNoTracking()
                .Where(a => a.Id == accountId)
                .ProjectToType<AccountResponse>()
                .FirstOrDefaultAsync(ct);
        }
    }
}
