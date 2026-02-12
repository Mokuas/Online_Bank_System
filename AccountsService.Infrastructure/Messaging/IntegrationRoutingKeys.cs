
namespace AccountsService.Infrastructure.Messaging
{
    public static class IntegrationRoutingKeys
    {
        public const string AccountCreated = "accounts.account.created";
        public const string AccountStatusChanged = "accounts.account.status.changed";
    }
}
