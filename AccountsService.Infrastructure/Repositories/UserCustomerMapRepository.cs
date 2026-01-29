using AccountsService.Application.Repositories;
using AccountsService.Domain.Entities;
using AccountsService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Repositories
{
    public sealed class UserCustomerMapRepository(AccountsDbContext db) : IUserCustomerMapRepository
    {
        public Task<UserCustomerMap?> GetByUserIdAsync(int userId)
            => db.UserCustomerMaps.FirstOrDefaultAsync(x => x.UserId == userId);

        public Task AddAsync(UserCustomerMap map)
            => db.UserCustomerMaps.AddAsync(map).AsTask();

        public Task SaveChangesAsync()
            => db.SaveChangesAsync();
    }
}
