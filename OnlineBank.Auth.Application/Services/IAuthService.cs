namespace OnlineBank.Auth.Application.Services
{
    using OnlineBank.Auth.Application.Dtos;

    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<bool> LoginAsync(LoginRequest request);
    }
}
