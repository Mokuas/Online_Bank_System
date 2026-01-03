using OnlineBank.Auth.Application.Dtos;
using OnlineBank.Auth.Application.Common;

namespace OnlineBank.Auth.Application.Services
{
    public interface IAuthService
    {
        Task<Result> RegisterAsync(RegisterRequest request);
        Task<Result> LoginAsync(LoginRequest request);
    }
}
