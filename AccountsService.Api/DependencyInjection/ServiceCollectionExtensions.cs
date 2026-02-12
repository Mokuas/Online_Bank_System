using AccountsService.Api.Errors;
using AccountsService.Api.Security;
using FluentValidation;
using FluentValidation.AspNetCore;
using AccountsService.Application.Validation.Accounts;
using AccountsService.Application.Security;
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

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<OpenAccountRequestValidator>();

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
