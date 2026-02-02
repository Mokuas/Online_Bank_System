using AccountsService.Domain.Entities;

namespace AccountsService.Application.Repositories
{
    public interface IUserCustomerMapRepository
    {
        Task<UserCustomerMap?> GetByUserIdAsync(int userId, CancellationToken ct);
        Task AddAsync(UserCustomerMap map, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}