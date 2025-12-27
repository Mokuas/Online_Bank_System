namespace OnlineBank.Auth.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;
    using OnlineBank.Auth.Domain.Entities;

    public class OnlineBankDbContext : DbContext
    {
        public OnlineBankDbContext(DbContextOptions<OnlineBankDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
