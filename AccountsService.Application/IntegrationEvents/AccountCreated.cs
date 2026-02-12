
namespace AccountsService.Application.IntegrationEvents
{
    public sealed record AccountCreated(
        int AccountId,
        int CustomerId,
        string AccountNumber,
        string Currency);
}
