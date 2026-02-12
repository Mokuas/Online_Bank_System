
namespace AccountsService.Infrastructure.Messaging
{
    public sealed class UnknownRoutingKeyException : Exception
    {
        public UnknownRoutingKeyException(string routingKey)
            : base($"Unknown routing key: '{routingKey}'.")
        {
        }
    }
}
