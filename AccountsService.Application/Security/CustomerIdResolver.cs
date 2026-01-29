using AccountsService.Application.Repositories;

namespace AccountsService.Application.Security
{
    public sealed class CustomerIdResolver(ICurrentUserService currentUser, IUserCustomerMapRepository maps) : ICustomerIdResolver
    {
        public async Task<int?> ResolveAsync(CancellationToken ct = default)
        {
            var userId = currentUser.UserId;
            if (userId is null)
                return null;

            var map = await maps.GetByUserIdAsync(userId.Value);
            return map?.CustomerId;
        }
    }
}
