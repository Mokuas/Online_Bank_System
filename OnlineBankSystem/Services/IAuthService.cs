namespace OnlineBankSystem.Services
{
    using OnlineBankSystem.Dtos;

    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<bool> LoginAsync(LoginRequest request);
    }
}
