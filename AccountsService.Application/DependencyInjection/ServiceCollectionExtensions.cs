using AccountsService.Application.IntegrationEvents;
using AccountsService.Application.IntegrationHandlers;
using AccountsService.Application.Messaging;
using AccountsService.Application.Services;
using AccountsService.Application.Validation.Accounts;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace AccountsService.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountsApplication(this IServiceCollection services)
        {
            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            services.AddScoped<IMapper, Mapper>();

            services.AddScoped<IIntegrationEventHandler<CustomerProfileCreated>, CustomerProfileCreatedHandler>();

            // Scan mappings from Application assembly
            TypeAdapterConfig.GlobalSettings.Scan(typeof(AccountService).Assembly);

            services.AddScoped<IAccountService, AccountService>();

            return services;
        }
    }
}
