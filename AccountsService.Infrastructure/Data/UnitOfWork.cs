using AccountsService.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Data
{
    public sealed class UnitOfWork(AccountsDbContext db) : IUnitOfWork
    {
        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
        {
            var tx = await db.Database.BeginTransactionAsync(ct);
            return new EfTransaction(tx);
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => db.SaveChangesAsync(ct);
    }
}
