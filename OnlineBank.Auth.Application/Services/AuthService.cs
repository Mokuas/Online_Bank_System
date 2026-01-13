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
            if (!Validation.IsValidEmail(request.Email))
                return Result.Failure("Invalid email format.");

            if (!Validation.IsValidPassword(request.Password))
                return Result.Failure("Password must be at least 8 characters.");

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

            if (!Validation.IsValidEmail(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return Result<LoginResponse>.Failure("Invalid email or password.");

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
                return Result<LoginResponse>.Failure("Invalid email or password.");

            if (!user.IsActive)
                return Result<LoginResponse>.Failure("User is inactive.");

            bool ok = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!ok)
                return Result<LoginResponse>.Failure("Invalid email or password.");

            var token = _tokenService.CreateAccessToken(user);
            return Result<LoginResponse>.Success(new LoginResponse(token));

        }

        public async Task<Result<List<AdminUserDto>>> GetUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            var dtos = users.Select(u => new AdminUserDto(
                u.Id,
                u.Email,
                u.Role,
                u.IsActive,
                u.CreatedAt
            )).ToList();

            return Result<List<AdminUserDto>>.Success(dtos);
        }

        public async Task<Result> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
                return Result.Failure("User not found.");

            if (request.Role is not null)
            {
                // Basic role guard (I'll improve validation later)
                var allowed = new[] { "Customer", "Employee", "Admin" };
                if (!allowed.Contains(request.Role))
                    return Result.Failure("Invalid role.");

                user.Role = request.Role;
            }

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            await _userRepository.SaveChangesAsync();
            return Result.Success();
        }


    }
}
