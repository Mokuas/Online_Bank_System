using AccountsService.Domain.Entities;
using AccountsService.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.Data
{
    public sealed class AccountsDbContext : DbContext
    {
        public AccountsDbContext(DbContextOptions<AccountsDbContext> options): base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<UserCustomerMap> UserCustomerMaps { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(FieldConstraints.AccountNumberMaxLength);

                entity.HasIndex(a => a.AccountNumber)
                    .IsUnique();

                entity.Property(a => a.Currency)
                    .IsRequired()
                    .HasMaxLength(FieldConstraints.CurrencyMaxLength);

                entity.Property(a => a.Balance)
                    .HasPrecision(18, 2);

                entity.Property(a => a.Status)
                    .IsRequired();

                entity.Property(a => a.Type)
                    .IsRequired();

                entity.Property(a => a.CreatedAt)
                    .IsRequired();

                entity.HasIndex(a => a.CustomerId);
            });

            modelBuilder.Entity<UserCustomerMap>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.UserId)
                    .IsUnique();

                entity.Property(x => x.UserId)
                    .IsRequired();

                entity.Property(x => x.CustomerId)
                    .IsRequired();

                entity.Property(x => x.CreatedAt)
                    .IsRequired();
            });
        }
    }
}
