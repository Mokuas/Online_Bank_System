using AccountsService.Application.Repositories;
using AccountsService.Infrastructure.Data.Entities;
using AccountsService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Repositories
{
    public sealed class UserCustomerMapRepository(AccountsDbContext db) : IUserCustomerMapRepository
    {
        public async Task<int?> GetCustomerIdByUserIdAsync(int userId, CancellationToken ct)
        {
            return await db.UserCustomerMaps
                .Where(x => x.UserId == userId)
                .Select(x => (int?)x.CustomerId)
                .FirstOrDefaultAsync(ct);
        }

        public async Task AddAsync(int userId, int customerId, CancellationToken ct)
        {
            var map = new UserCustomerMap(userId, customerId);
            await db.UserCustomerMaps.AddAsync(map, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct)
            => db.SaveChangesAsync(ct);
    }
}
