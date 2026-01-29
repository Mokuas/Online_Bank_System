
namespace AccountsService.Domain.Entities
{
    public sealed class UserCustomerMap
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
