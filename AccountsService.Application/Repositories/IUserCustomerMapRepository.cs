using AccountsService.Domain.Entities;

namespace AccountsService.Application.Repositories
{
    public interface IUserCustomerMapRepository
    {
        Task<UserCustomerMap?> GetByUserIdAsync(int userId);
        Task AddAsync(UserCustomerMap map);
        Task SaveChangesAsync();
    }
}