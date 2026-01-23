namespace OnlineBank.Auth.Application.Security
{
    public interface ITokenBlacklistService
    {
        Task RevokeAsync(string jti, int userId, DateTime expiresAtUtc);
        Task<bool> IsRevokedAsync(string jti);
    }
}
