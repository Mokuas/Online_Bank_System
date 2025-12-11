using OnlineBankSystem.Entities;
using OnlineBankSystem.Dtos;
using OnlineBankSystem.Repositories;
using Microsoft.AspNetCore.Identity;

namespace OnlineBankSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;           
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
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

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LoginAsync(LoginRequest request)
        {

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return false;

            if (!user.IsActive) 
                return false;

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            return verificationResult == PasswordVerificationResult.Success;
        }

    }
}
