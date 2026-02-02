using AccountsService.Application.Repositories;
using AccountsService.Domain.Entities;
using AccountsService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Repositories
{
    public sealed class UserCustomerMapRepository(AccountsDbContext db) : IUserCustomerMapRepository
    {
        public Task<UserCustomerMap?> GetByUserIdAsync(int userId, CancellationToken ct)
            => db.UserCustomerMaps.FirstOrDefaultAsync(x => x.UserId == userId, ct);

        public Task AddAsync(UserCustomerMap map, CancellationToken ct)
            => db.UserCustomerMaps.AddAsync(map, ct).AsTask();

        public Task SaveChangesAsync(CancellationToken ct)
            => db.SaveChangesAsync(ct);
    }
}
