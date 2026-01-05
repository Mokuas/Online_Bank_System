using OnlineBank.Auth.Application.Common;
using OnlineBank.Auth.Application.Dtos;

namespace OnlineBank.Auth.Application.Services
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequest request);
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    }
}
