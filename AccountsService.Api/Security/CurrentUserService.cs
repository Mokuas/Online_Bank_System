using AccountsService.Application.Security;
using System.Security.Claims;

namespace AccountsService.Api.Security
{
    public sealed class CurrentUserService(IHttpContextAccessor http) : ICurrentUserService
    {
        private ClaimsPrincipal? User => http.HttpContext?.User;

        public int? UserId =>
            int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
                ? id
                : null;

        public string? Email => User?.FindFirstValue(ClaimTypes.Email);

        public string? Role => User?.FindFirstValue(ClaimTypes.Role);
    }
}
