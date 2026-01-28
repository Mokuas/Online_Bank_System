using Microsoft.EntityFrameworkCore;
using ProfilesService.Application.Repositories;
using ProfilesService.Domain.Entities;
using ProfilesService.Domain.Enums;
using ProfilesService.Infrastructure.Data;

namespace ProfilesService.Infrastructure.Repositories
{
    public sealed class CustomerRepository(ProfilesDbContext db) : ICustomerRepository
    {
        private readonly ProfilesDbContext _db = db;

        public Task<Customer?> GetByIdAsync(int id)
        {
            return _db.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Customer?> GetByIdReadAsync(int id)
        {
            return _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<Customer?> GetByUserIdAsync(int userId)
        {
            return _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public Task<Customer?> GetByUserIdReadAsync(int userId)
        {
            return _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public Task<bool> UserIdExistsAsync(int userId)
        {
            return _db.Customers.AnyAsync(c => c.UserId == userId);
        }

        public async Task AddAsync(Customer customer)
        {
            await _db.Customers.AddAsync(customer);
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Customer>> GetByKycStatusAsync(KycStatus kycStatus)
        {
            return await _db.Customers
                .AsNoTracking()
                .Where(c => c.KycStatus == kycStatus)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }
    }
}