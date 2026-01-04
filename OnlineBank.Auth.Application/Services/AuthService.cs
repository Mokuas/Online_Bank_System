using OnlineBank.Auth.Application.Common;
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
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
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

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Result<LoginResponse>.Failure("Invalid email or password.");

            if (!user.IsActive)
                return Result<LoginResponse>.Failure("User is inactive.");

            bool ok = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!ok)
                return Result<LoginResponse>.Failure("Invalid email or password.");

            var token = _tokenService.CreateAccessToken(user);
            return Result<LoginResponse>.Success(new LoginResponse { AccessToken = token });
        }
    }
}
