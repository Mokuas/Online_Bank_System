using AccountsService.Application.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountsService.Infrastructure.Data
{
    internal sealed class EfTransaction(IDbContextTransaction tx) : ITransaction
    {
        public Task CommitAsync(CancellationToken ct) => tx.CommitAsync(ct);
        public ValueTask DisposeAsync() => tx.DisposeAsync();
    }
}
