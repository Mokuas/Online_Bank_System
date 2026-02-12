using AccountsService.Application.Messaging;
using AccountsService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Messaging
{
    public sealed class OutboxPublisherHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxPublisherHostedService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
                    var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                    var batch = await db.OutboxMessages
                        .Where(x => x.ProcessedAtUtc == null && x.Attempts < 20)
                        .OrderBy(x => x.Id)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var msg in batch)
                    {
                        try
                        {
                            using var doc = JsonDocument.Parse(msg.PayloadJson);
                            await bus.PublishAsync(msg.RoutingKey, doc.RootElement, stoppingToken);

                            msg.ProcessedAtUtc = DateTime.UtcNow;
                            msg.LastError = null;
                        }
                        catch (Exception ex)
                        {
                            msg.Attempts += 1;
                            msg.LastError = ex.Message.Length > 4000 ? ex.Message[..4000] : ex.Message;

                            logger.LogError(ex, "Outbox publish failed. OutboxId={OutboxId}", msg.Id);
                        }
                    }

                    if (batch.Count > 0)
                        await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Outbox loop failed.");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }
}
