
namespace AccountsService.Application.Repositories
{
    public interface IUserCustomerMapRepository
    {
        Task<int?> GetCustomerIdByUserIdAsync(int userId, CancellationToken ct);
        Task AddAsync(int userId, int customerId, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}