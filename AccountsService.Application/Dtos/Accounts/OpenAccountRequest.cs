using AccountsService.Domain.Enums;

namespace AccountsService.Application.Dtos.Accounts
{
    public sealed record OpenAccountRequest(
    int CustomerId,
    AccountType Type,
    string Currency);
}
