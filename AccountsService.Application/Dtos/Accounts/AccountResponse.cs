using AccountsService.Domain.Enums;

namespace AccountsService.Application.Dtos.Accounts
{
    public sealed record AccountResponse(
    int Id,
    int CustomerId,
    string AccountNumber,
    AccountType Type,
    string Currency,
    decimal Balance,
    AccountStatus Status,
    DateTime CreatedAt);
}
