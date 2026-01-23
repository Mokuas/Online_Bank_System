namespace OnlineBank.Auth.Application.Dtos
{
    public sealed record UpdateUserRequest(
        string? Role,
        bool? IsActive
    );
}
