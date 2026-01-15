namespace OnlineBank.Auth.Domain.Entities
{
    public class RevokedToken
    {
        public int Id { get; set; }

        public string Jti { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime RevokedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
