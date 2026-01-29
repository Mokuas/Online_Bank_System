using RabbitMQ.Client;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class RabbitMqConnectionProvider : IAsyncDisposable
    {
        public IConnection? Connection { get; private set; }
        public IChannel? Channel { get; private set; }

        public async Task InitializeAsync(RabbitMqOptions options, CancellationToken ct)
        {
            if (Connection is not null)
                return;

            var factory = new ConnectionFactory
            {
                HostName = options.Host,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            Connection = await factory.CreateConnectionAsync(ct);
            Channel = await Connection.CreateChannelAsync();

            await Channel.ExchangeDeclareAsync(
                exchange: options.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (Channel is not null)
                await Channel.CloseAsync();

            if (Connection is not null)
                await Connection.CloseAsync();
        }
    }
}
