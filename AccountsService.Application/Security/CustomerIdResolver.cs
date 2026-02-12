using AccountsService.Application.Repositories;

namespace AccountsService.Application.Security
{
    public sealed class CustomerIdResolver(ICurrentUserService currentUser, IUserCustomerMapRepository userCustomerMapRepository) : ICustomerIdResolver
    {
        public async Task<int?> ResolveAsync(CancellationToken ct = default)
        {
            var userId = currentUser.UserId;
            if (userId is null)
                return null;

            return await userCustomerMapRepository.GetCustomerIdByUserIdAsync(userId.Value, ct);
        }
    }
}
