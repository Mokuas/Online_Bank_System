
namespace AccountsService.Application.IntegrationEvents
{
    public sealed record CustomerProfileCreated(
         int CustomerId,
         int UserId,
         string Email);
}
