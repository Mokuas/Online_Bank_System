namespace OnlineBank.Auth.Application.Dtos
{
    public sealed record AdminUserDto(
        int Id,
        string Email,
        string Role,
        bool IsActive,
        DateTime CreatedAt
    );
}
