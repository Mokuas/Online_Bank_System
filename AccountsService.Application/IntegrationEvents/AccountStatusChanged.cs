using AccountsService.Domain.Enums;

namespace AccountsService.Application.IntegrationEvents
{
    public sealed record AccountStatusChanged(
        int AccountId,
        AccountStatus Status);
}
