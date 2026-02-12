using AccountsService.Application.Common;
using AccountsService.Application.Dtos.Accounts;
using AccountsService.Domain.Enums;

namespace AccountsService.Application.Services
{
    public interface IAccountService
    {
        Task<Result<AccountResponse>> OpenAsync(OpenAccountRequest request, CancellationToken ct);
        Task<Result<IReadOnlyList<AccountResponse>>> GetMeAsync(CancellationToken ct);
        Task<Result<AccountResponse>> GetByIdAsync(int id, CancellationToken ct);
        Task<Result<IReadOnlyList<AccountResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken ct);
        Task<Result<AccountResponse>> ChangeStatusAsync(int id, ChangeAccountStatusRequest request, CancellationToken ct);
        Task<Result<BalanceResponse>> GetBalanceAsync(int id, CancellationToken ct);
    }
}
