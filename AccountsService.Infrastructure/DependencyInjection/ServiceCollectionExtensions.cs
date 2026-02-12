using AccountsService.Application.Common;
using AccountsService.Application.Messaging;
using AccountsService.Application.Repositories;
using AccountsService.Infrastructure.Data;
using AccountsService.Infrastructure.Messaging;
using AccountsService.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AccountsService.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountsInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AccountsDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AccountsDb")));

            services.AddOptions<RabbitMqOptions>()
                .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
                .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "RabbitMq:Host is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Username), "RabbitMq:Username is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "RabbitMq:Password is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Exchange), "RabbitMq:Exchange is required")
                .Validate(o => !string.IsNullOrWhiteSpace(o.Queue), "RabbitMq:Queue is required")
                .ValidateOnStart();

            services.AddSingleton<IIntegrationEventRouting, IntegrationEventRouting>();

            services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

            services.AddScoped<IOutboxWriter, OutboxWriter>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHostedService<OutboxPublisherHostedService>();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserCustomerMapRepository, UserCustomerMapRepository>();
            services.AddScoped<IAccountQueries, AccountQueries>();

            services.AddSingleton<RabbitMqConnectionProvider>();
            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddHostedService<RabbitMqPublisherHostedService>();

            services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
            services.AddHostedService<RabbitMqConsumerHostedService>();

            return services;
        }
    }
}
