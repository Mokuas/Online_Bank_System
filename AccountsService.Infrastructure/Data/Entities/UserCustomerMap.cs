
namespace AccountsService.Infrastructure.Data.Entities
{
    public sealed class UserCustomerMap
    {
        public int Id { get; private set; }

        public int UserId { get; private set; }
        public int CustomerId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private UserCustomerMap() { }

        public UserCustomerMap(int userId, int customerId)
        {
            UserId = userId;
            CustomerId = customerId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
