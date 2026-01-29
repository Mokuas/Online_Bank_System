using AccountsService.Api.Errors;
using AccountsService.Api.Security;
using AccountsService.Application.IntegrationHandlers;
using AccountsService.Application.Mapper;
using AccountsService.Application.Messaging;
using AccountsService.Application.Repositories;
using AccountsService.Application.Security;
using AccountsService.Application.Services;
using AccountsService.Application.Validation.Accounts;
using AccountsService.Application.IntegrationEvents;
using AccountsService.Infrastructure.Data;
using AccountsService.Infrastructure.Messaging;
using AccountsService.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AccountsService.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAccountsApi(this IServiceCollection services)
        {
            services.AddOpenApi();

            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(kvp => kvp.Value?.Errors.Count > 0)
                            .ToDictionary(
                                kvp => kvp.Key,
                                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                            );

                        var payload = new ApiErrorResponse(
                            Code: "VALIDATION_ERROR",
                            Message: "One or more validation errors occurred.",
                            Errors: errors
                        );

                        return new BadRequestObjectResult(payload);
                    };
                });

            return services;
        }

        public static IServiceCollection AddAccountsApplication(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<OpenAccountRequestValidator>();

            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddScoped<IIntegrationEventHandler<CustomerProfileCreated>,CustomerProfileCreatedHandler>();

            TypeAdapterConfig.GlobalSettings.Scan(typeof(AccountMappingConfig).Assembly);

            services.AddScoped<IAccountService, AccountService>();

            return services;
        }

        public static IServiceCollection AddAccountsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AccountsDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AccountsDb")));

            services.Configure<RabbitMqOptions>(
                configuration.GetSection(RabbitMqOptions.SectionName));

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserCustomerMapRepository, UserCustomerMapRepository>();
            services.AddScoped<ICustomerIdResolver, CustomerIdResolver>();
            services.AddSingleton<RabbitMqConnectionProvider>();
            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddHostedService<RabbitMqPublisherHostedService>();
            services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
            services.AddHostedService<RabbitMqConsumerHostedService>();

            return services;
        }

        public static IServiceCollection AddAccountsCurrentUser(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
         
            return services;
        }

        public static IServiceCollection AddAccountsJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("Jwt options not configured.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),

                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
