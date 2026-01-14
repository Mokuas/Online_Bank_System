using ProfilesService.Domain.Enums;

namespace ProfilesService.Application.Dtos.Customers;

public sealed record UpdateKycStatusRequest(KycStatus KycStatus);
