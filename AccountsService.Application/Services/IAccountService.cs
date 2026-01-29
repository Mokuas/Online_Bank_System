using AccountsService.Application.Common;
using AccountsService.Application.Dtos.Accounts;
using AccountsService.Domain.Enums;

namespace AccountsService.Application.Services
{
    public interface IAccountService
    {
        Task<Result<AccountResponse>> OpenAsync(OpenAccountRequest request);
        Task<Result<IReadOnlyList<AccountResponse>>> GetMeAsync();
        Task<Result<AccountResponse>> GetByIdAsync(int id);
        Task<Result<IReadOnlyList<AccountResponse>>> GetByCustomerIdAsync(int customerId);
        Task<Result<AccountResponse>> ChangeStatusAsync(int id, ChangeAccountStatusRequest request);
        Task<Result<BalanceResponse>> GetBalanceAsync(int id);
    }
}
