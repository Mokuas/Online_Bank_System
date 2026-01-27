using ProfilesService.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddProfilesApi()
    .AddProfilesCurrentUser()
    .AddProfilesApplication()
    .AddProfilesInfrastructure(builder.Configuration)
    .AddProfilesJwtAuth(builder.Configuration);

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
