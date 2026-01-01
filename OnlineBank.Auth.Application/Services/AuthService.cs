using OnlineBank.Auth.Application.Dtos;
using OnlineBank.Auth.Application.Repositories;
using OnlineBank.Auth.Application.Security;
using OnlineBank.Auth.Domain.Entities;

namespace OnlineBank.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                return false;

            var user = new User
            {
                Email = request.Email,
                Role = "Customer",
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.Hash(request.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null) return false;
            if (!user.IsActive) return false;

            return _passwordHasher.Verify(request.Password, user.PasswordHash);
        }
    }
}
