
namespace AccountsService.Application.Messaging
{
    public static class RoutingKeys
    {
        // Incoming (from Profiles service)
        public const string CustomerProfileCreated = "profiles.customer.created";

        // Outgoing (from Accounts service)
        public const string AccountCreated = "accounts.account.created";
        public const string AccountStatusChanged = "accounts.account.status.changed";
    }
}
