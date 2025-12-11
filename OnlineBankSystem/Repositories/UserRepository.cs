using Microsoft.EntityFrameworkCore;
using OnlineBankSystem.Data;
using OnlineBankSystem.Entities;

namespace OnlineBankSystem.Repositories
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
    }
}
