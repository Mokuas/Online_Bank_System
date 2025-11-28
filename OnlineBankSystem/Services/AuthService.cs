using OnlineBankSystem.Data;
using OnlineBankSystem.Entities;
using Microsoft.EntityFrameworkCore;
using OnlineBankSystem.Dtos;

namespace OnlineBankSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly OnlineBankDbContext _context;

        public AuthService(OnlineBankDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            // Email var mı?
            bool exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (exists) return false;

            var user = new User
            {
                Email = request.Email,
                PasswordHash = request.Password // Not secure – JWT ekleyince hash yapacağız
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) return false;

            if (user.PasswordHash != request.Password) return false;

            return true;
        }
    }
}
