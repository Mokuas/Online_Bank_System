using AccountsService.Domain.Enums;

namespace AccountsService.Domain.Entities
{
    public sealed class Account
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        public string AccountNumber { get; set; } = string.Empty;
        public AccountType Type { get; set; }

        public string Currency { get; set; } = "USD";
        public decimal Balance { get; set; }

        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
