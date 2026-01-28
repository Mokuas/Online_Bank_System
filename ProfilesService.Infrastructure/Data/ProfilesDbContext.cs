using Microsoft.EntityFrameworkCore;
using ProfilesService.Domain.Entities;
using ProfilesService.Application.Common;

namespace ProfilesService.Infrastructure.Data
{
    public class ProfilesDbContext(DbContextOptions<ProfilesDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {

                entity.HasKey(c => c.Id);

                entity.HasIndex(c => c.UserId).IsUnique();

                entity.Property(c => c.UserId)
                    .IsRequired();

                entity.Property(c => c.FirstName)
                    .IsRequired()
                    .HasMaxLength(FieldConstraints.NameMaxLength);

                entity.Property(c => c.LastName)
                    .IsRequired()
                    .HasMaxLength(FieldConstraints.NameMaxLength);

                entity.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(FieldConstraints.AddressMaxLength);

                entity.Property(c => c.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(FieldConstraints.PhoneNumberMaxLength);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(FieldConstraints.EmailMaxLength);

                entity.Property(c => c.DateOfBirth)
                    .IsRequired();

                entity.Property(c => c.KycStatus)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(c => c.CreatedAt)
                    .IsRequired();
            });
        }
    }
}
