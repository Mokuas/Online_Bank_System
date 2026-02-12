using System.ComponentModel.DataAnnotations;

namespace AccountsService.Infrastructure.Data
{
    public sealed class OutboxMessage
    {
        public long Id { get; set; }

        [MaxLength(200)]
        public string RoutingKey { get; set; } = default!;

        [MaxLength(200)]
        public string EventType { get; set; } = default!;

        public string PayloadJson { get; set; } = default!;

        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAtUtc { get; set; }

        public int Attempts { get; set; }

        public string? LastError { get; set; }

        // Concurrency token (recommended)
        [Timestamp]
        public byte[] RowVersion { get; set; } = default!;
    }
}
