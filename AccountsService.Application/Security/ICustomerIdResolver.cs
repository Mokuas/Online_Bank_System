
namespace AccountsService.Application.Security
{
    public interface ICustomerIdResolver
    {
        Task<int?> ResolveAsync(CancellationToken ct = default);
    }
}
