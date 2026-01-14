using ProfilesService.Domain.Enums;

namespace ProfilesService.Application.Dtos.Customers;

public sealed record CustomerResponse(
    int Id,
    int UserId,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Address,
    string PhoneNumber,
    string Email,
    KycStatus KycStatus,
    DateTime CreatedAt
);
