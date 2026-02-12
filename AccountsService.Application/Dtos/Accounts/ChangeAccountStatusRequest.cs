using AccountsService.Domain.Enums;

namespace AccountsService.Application.Dtos.Accounts
{
    public sealed record ChangeAccountStatusRequest(AccountStatus Status);
}
