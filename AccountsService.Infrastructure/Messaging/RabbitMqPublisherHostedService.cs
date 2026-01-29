using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class RabbitMqPublisherHostedService(RabbitMqConnectionProvider provider, IOptions<RabbitMqOptions> options) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await provider.InitializeAsync(options.Value, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
