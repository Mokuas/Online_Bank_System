
namespace AccountsService.Application.Dtos.Accounts
{
    public sealed record BalanceResponse(int AccountId, decimal Balance, string Currency);
}
