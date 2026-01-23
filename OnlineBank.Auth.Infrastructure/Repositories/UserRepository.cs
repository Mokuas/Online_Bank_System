using Microsoft.EntityFrameworkCore;
using OnlineBank.Auth.Infrastructure.Data;
using OnlineBank.Auth.Domain.Entities;
using OnlineBank.Auth.Application.Repositories;


namespace OnlineBank.Auth.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly OnlineBankDbContext _context;

        public UserRepository(OnlineBankDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            return _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Id)
                .ToListAsync();
        }

    }
}
