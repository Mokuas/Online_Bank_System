using ProfilesService.Application.Common;
using ProfilesService.Application.Dtos.Customers;
using ProfilesService.Domain.Enums;

namespace ProfilesService.Application.Services;

public interface ICustomerService
{
    Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request);
    Task<Result<CustomerResponse>> GetMeAsync();
    Task<Result<CustomerResponse>> GetByIdAsync(int id);
    Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request);
    Task<Result<IReadOnlyList<CustomerResponse>>> GetByKycStatusAsync(KycStatus kycStatus);
    Task<Result<CustomerResponse>> UpdateKycStatusAsync(int id, UpdateKycStatusRequest request);
}
