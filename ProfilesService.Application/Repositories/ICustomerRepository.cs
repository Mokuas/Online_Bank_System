using ProfilesService.Domain.Entities;
using ProfilesService.Domain.Enums;

namespace ProfilesService.Application.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer?> GetByIdReadAsync(int id);

    Task<Customer?> GetByUserIdAsync(int userId);
    Task<Customer?> GetByUserIdReadAsync(int userId);

    Task<bool> UserIdExistsAsync(int userId);

    Task AddAsync(Customer customer);

    Task SaveChangesAsync();

    Task<IReadOnlyList<Customer>> GetByKycStatusAsync(KycStatus kycStatus);
}
