using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class RabbitMqPublisherHostedService(RabbitMqConnectionProvider provider, IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisherHostedService> logger) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var opts = options.Value;
                await provider.InitializeAsync(opts, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RabbitMQ not reachable on startup. App will continue; publishing will retry.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
