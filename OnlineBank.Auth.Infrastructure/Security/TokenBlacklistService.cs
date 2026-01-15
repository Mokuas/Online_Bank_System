using Microsoft.EntityFrameworkCore;
using OnlineBank.Auth.Application.Security;
using OnlineBank.Auth.Domain.Entities;
using OnlineBank.Auth.Infrastructure.Data;

namespace OnlineBank.Auth.Infrastructure.Security
{
    public sealed class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly OnlineBankDbContext _db;

        public TokenBlacklistService(OnlineBankDbContext db)
        {
            _db = db;
        }

        public async Task RevokeAsync(string jti, int userId, DateTime expiresAtUtc)
        {
            // Prevent duplicates (unique index also protects us)
            bool exists = await _db.RevokedTokens.AnyAsync(x => x.Jti == jti);
            if (exists) return;

            var revoked = new RevokedToken
            {
                Jti = jti,
                UserId = userId,
                ExpiresAtUtc = expiresAtUtc,
                RevokedAtUtc = DateTime.UtcNow
            };

            await _db.RevokedTokens.AddAsync(revoked);
            await _db.SaveChangesAsync();
        }

        public Task<bool> IsRevokedAsync(string jti)
        {
            return _db.RevokedTokens.AnyAsync(x => x.Jti == jti);
        }
    }
}
