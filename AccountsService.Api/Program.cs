using AccountsService.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAccountsApi()
    .AddAccountsCurrentUser()
    .AddAccountsApplication()
    .AddAccountsInfrastructure(builder.Configuration)
    .AddAccountsJwtAuth(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
