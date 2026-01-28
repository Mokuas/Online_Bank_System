using ProfilesService.Domain.Enums;

namespace ProfilesService.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public KycStatus KycStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
