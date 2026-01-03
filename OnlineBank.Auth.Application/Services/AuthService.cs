using OnlineBank.Auth.Application.Dtos;
using OnlineBank.Auth.Application.Repositories;
using OnlineBank.Auth.Application.Security;
using OnlineBank.Auth.Domain.Entities;
using OnlineBank.Auth.Application.Common;

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

        public async Task<Result> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
                return Result.Failure("Email already exists.");

            var user = new User
            {
                Email = request.Email,
                Role = "Customer",
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.Hash(request.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Result.Failure("Invalid email or password.");

            if (!user.IsActive)
                return Result.Failure("User is inactive.");

            bool ok = _passwordHasher.Verify(request.Password, user.PasswordHash);
            return ok ? Result.Success() : Result.Failure("Invalid email or password.");
        }
    }
}
