using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProfilesService.Infrastructure.Data
{
    public sealed class ProfilesDbContextFactory : IDesignTimeDbContextFactory<ProfilesDbContext>
    {
        public ProfilesDbContext CreateDbContext(string[] args)
        {
  
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ProfilesService.Api");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("ProfilesDb");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'ProfilesDb' was not found.");

            var optionsBuilder = new DbContextOptionsBuilder<ProfilesDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ProfilesDbContext(optionsBuilder.Options);
        }
    }
}
